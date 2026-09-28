<#
.SYNOPSIS
  Deploys the wUtility Web public demo to Azure (App Service + Azure SQL free offer).

.DESCRIPTION
  1. Creates/updates the resource group and infra/main.bicep (idempotent).
  2. Opens the SQL firewall to this machine, runs web/db/*.sql through
     tools/DbSetup as you (the SQL server's Entra admin), grants the web app's
     managed identity its per-database roles, then closes the firewall again.
  3. Builds Angular + publishes the API into one zip and deploys it.
  4. Smoke-tests the live site.

  Needs: Azure CLI logged in (az login), .NET 10 SDK, Node. Secrets are
  generated once into .secrets.json next to this script (gitignored) and reused,
  so re-running never rotates the vault key out from under stored data.

.EXAMPLE
  ./deploy.ps1 -AppName wutility-demo-ak -WhatIf     # preview the infra changes only
  ./deploy.ps1 -AppName wutility-demo-ak             # full deploy
  ./deploy.ps1 -AppName wutility-demo-ak -CodeOnly   # just rebuild and redeploy the app
#>
[CmdletBinding()]
param(
    [Parameter(Mandatory)] [string] $AppName,
    [string] $ResourceGroup = 'wutility-demo-rg',
    [string] $Location = 'canadacentral',
    [ValidateSet('F1', 'B1')] [string] $PlanSku = 'F1',
    [switch] $WhatIf,
    [switch] $CodeOnly
)

$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true
$webRoot = Resolve-Path (Join-Path $PSScriptRoot '..\..')
$secretsPath = Join-Path $PSScriptRoot '.secrets.json'

function Step($text) { Write-Host "`n==> $text" -ForegroundColor Cyan }

Step 'Checking Azure CLI login'
if (-not (Get-Command az -ErrorAction SilentlyContinue)) {
    throw 'Azure CLI not found. Install it: winget install -e --id Microsoft.AzureCLI'
}
$account = az account show --query '{name:name, id:id}' -o json 2>$null | ConvertFrom-Json
if (-not $account) { throw 'Not logged in. Run: az login' }
Write-Host "Subscription: $($account.name) ($($account.id))"

$sqlServerName = "$AppName-sql"

if (-not $CodeOnly) {
    Step 'Loading or creating deployment secrets'
    if (Test-Path $secretsPath) {
        $secrets = Get-Content $secretsPath -Raw | ConvertFrom-Json
    } else {
        $bytes = [byte[]]::new(32)
        [System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
        $vaultKey = [Convert]::ToBase64String($bytes)
        [System.Security.Cryptography.RandomNumberGenerator]::Fill($bytes)
        $apiKey = [Convert]::ToBase64String($bytes).TrimEnd('=').Replace('+', '-').Replace('/', '_')
        $secrets = [pscustomobject]@{ apiKey = $apiKey; connectionVaultKey = $vaultKey }
        $secrets | ConvertTo-Json | Set-Content $secretsPath -Encoding utf8
        Write-Host "Created $secretsPath (gitignored; keep it, the vault key can't be recovered)."
    }

    $admin = az ad signed-in-user show --query '{id:id, upn:userPrincipalName}' -o json | ConvertFrom-Json
    $myIp = (Invoke-RestMethod -Uri 'https://api.ipify.org' -TimeoutSec 15).Trim()
    Write-Host "SQL Entra admin: $($admin.upn)   deployer IP: $myIp"

    Step "Resource group $ResourceGroup ($Location)"
    az group create --name $ResourceGroup --location $Location -o none

    # Secure parameters go through a temp file rather than the command line.
    $paramFile = New-TemporaryFile
    try {
        @{
            '$schema'      = 'https://schema.management.azure.com/schemas/2019-04-01/deploymentParameters.json#'
            contentVersion = '1.0.0.0'
            parameters     = @{
                location           = @{ value = $Location }
                appName            = @{ value = $AppName }
                sqlServerName      = @{ value = $sqlServerName }
                sqlAdminLogin      = @{ value = $admin.upn }
                sqlAdminObjectId   = @{ value = $admin.id }
                deployerIp         = @{ value = $myIp }
                planSku            = @{ value = $PlanSku }
                apiKey             = @{ value = $secrets.apiKey }
                connectionVaultKey = @{ value = $secrets.connectionVaultKey }
            }
        } | ConvertTo-Json -Depth 5 | Set-Content $paramFile -Encoding utf8

        $template = Join-Path $webRoot 'infra\main.bicep'
        if ($WhatIf) {
            Step 'What-if (no changes made)'
            az deployment group what-if --resource-group $ResourceGroup --template-file $template --parameters "@$paramFile"
            return
        }

        Step 'Deploying infra/main.bicep'
        $outputs = az deployment group create --resource-group $ResourceGroup --name "wutility-$(Get-Date -Format yyyyMMddHHmmss)" `
            --template-file $template --parameters "@$paramFile" --query properties.outputs -o json | ConvertFrom-Json
    } finally {
        Remove-Item $paramFile -ErrorAction SilentlyContinue
    }

    Step 'Creating demo schemas and granting the managed identity (tools/DbSetup)'
    try {
        dotnet run --project (Join-Path $webRoot 'tools\DbSetup') -- --server $outputs.sqlServerFqdn.value --identity $AppName
    } finally {
        # The firewall hole for this machine only exists while the scripts run.
        az sql server firewall-rule delete --resource-group $ResourceGroup --server $sqlServerName --name Deployer -o none
        Write-Host 'Removed the Deployer firewall rule.'
    }
}

Step 'Building Angular and publishing the API'
$out = Join-Path $webRoot 'out'
Remove-Item $out -Recurse -Force -ErrorAction SilentlyContinue
Push-Location (Join-Path $webRoot 'frontend')
try {
    npm ci --no-audit --no-fund
    npx ng build --configuration production
} finally { Pop-Location }
dotnet publish (Join-Path $webRoot 'api\SchemaSyncApi.csproj') -c Release -o (Join-Path $out 'app')
Copy-Item (Join-Path $webRoot 'frontend\dist\frontend\browser') (Join-Path $out 'app\wwwroot') -Recurse
$zip = Join-Path $out 'app.zip'
Compress-Archive -Path (Join-Path $out 'app\*') -DestinationPath $zip

Step "Deploying to $AppName"
az webapp deploy --resource-group $ResourceGroup --name $AppName --src-path $zip --type zip -o none

Step 'Smoke test'
$url = "https://$AppName.azurewebsites.net"
Invoke-RestMethod "$url/healthz" -TimeoutSec 120 | Out-Null
$demo = Invoke-RestMethod "$url/api/demo" -TimeoutSec 60
if (-not $demo.enabled) { throw 'Demo mode is not enabled on the deployed app.' }
# First SQL call may wait for a paused serverless database to resume.
$compare = Invoke-RestMethod "$url/api/compare" -Method Post -ContentType 'application/json' `
    -Body '{"sourceConnectionString":"","targetConnectionString":""}' -TimeoutSec 180
Write-Host "healthz ok; demo on ($($demo.sourceDatabase) -> $($demo.targetDatabase)); compare found $($compare.diffs.Count) diff(s)."
Write-Host "`nLive: $url" -ForegroundColor Green
