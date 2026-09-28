// wUtility Web public demo on Azure:
//   - App Service (Linux, .NET 10) serving the API and the Angular build from one origin
//   - Azure SQL logical server with Microsoft Entra-only authentication (no SQL passwords)
//   - the four demo databases on the Azure SQL Database free offer (serverless, auto-pause)
//   - the web app's system-assigned managed identity as its only way into SQL
//
// Deploy with deploy/azure/deploy.ps1, which also creates the database users
// for the managed identity (that part is T-SQL, which Bicep can't run).

@description('Region for all resources.')
param location string = 'canadacentral'

@description('Globally unique web app name; becomes https://<appName>.azurewebsites.net.')
param appName string

@description('Globally unique SQL server name; becomes <sqlServerName>.database.windows.net.')
param sqlServerName string = '${appName}-sql'

@description('Entra admin of the SQL server: user principal name (e.g. you@example.com).')
param sqlAdminLogin string

@description('Entra admin object id (az ad signed-in-user show --query id -o tsv).')
param sqlAdminObjectId string

@description('Public IP allowed through the SQL firewall so deploy.ps1 can run the setup scripts. Empty = none.')
param deployerIp string = ''

@description('App Service plan SKU. F1 is free (60 CPU minutes/day, no always-on); B1 is the smallest paid tier.')
@allowed([ 'F1', 'B1' ])
param planSku string = 'F1'

@secure()
@description('X-Api-Key for service-to-service calls (at least 32 chars). Unused by the browser in demo mode.')
param apiKey string

@secure()
@description('Base64 32-byte AES key for the connection vault.')
param connectionVaultKey string

var databases = [
  { name: 'SchemaSyncDemo_Source', setting: 'SchemaSyncSource' }
  { name: 'SchemaSyncDemo_Target', setting: 'SchemaSyncTarget' }
  { name: 'TotalsystemDemo', setting: 'Totalsystem' }
  { name: 'dbConfigDataBasesDemo', setting: 'DbConfig' }
]

resource sqlServer 'Microsoft.Sql/servers@2023-08-01' = {
  name: sqlServerName
  location: location
  properties: {
    minimalTlsVersion: '1.2'
    publicNetworkAccess: 'Enabled'
    administrators: {
      administratorType: 'ActiveDirectory'
      azureADOnlyAuthentication: true
      login: sqlAdminLogin
      sid: sqlAdminObjectId
      tenantId: subscription().tenantId
      principalType: 'User'
    }
  }
}

// 0.0.0.0 - 0.0.0.0 is Azure's convention for "allow Azure services", which
// is how App Service (without VNet integration, not available on F1) reaches SQL.
resource allowAzure 'Microsoft.Sql/servers/firewallRules@2023-08-01' = {
  parent: sqlServer
  name: 'AllowAzureServices'
  properties: {
    startIpAddress: '0.0.0.0'
    endIpAddress: '0.0.0.0'
  }
}

resource allowDeployer 'Microsoft.Sql/servers/firewallRules@2023-08-01' = if (!empty(deployerIp)) {
  parent: sqlServer
  name: 'Deployer'
  properties: {
    startIpAddress: deployerIp
    endIpAddress: deployerIp
  }
}

// Free offer: up to 10 databases per subscription, each with 100,000 vCore
// seconds and 32 GB a month. When the monthly budget runs out the database
// pauses until the next month instead of billing.
resource db 'Microsoft.Sql/servers/databases@2023-08-01' = [for d in databases: {
  parent: sqlServer
  name: d.name
  location: location
  sku: {
    name: 'GP_S_Gen5_2'
    tier: 'GeneralPurpose'
    family: 'Gen5'
    capacity: 2
  }
  properties: {
    useFreeLimit: true
    freeLimitExhaustionBehavior: 'AutoPause'
    autoPauseDelay: 60
    minCapacity: json('0.5')
    maxSizeBytes: 34359738368
    requestedBackupStorageRedundancy: 'Local'
  }
}]

resource plan 'Microsoft.Web/serverfarms@2023-12-01' = {
  name: '${appName}-plan'
  location: location
  kind: 'linux'
  sku: {
    name: planSku
  }
  properties: {
    reserved: true
  }
}

resource webApp 'Microsoft.Web/sites@2023-12-01' = {
  name: appName
  location: location
  kind: 'app,linux'
  identity: {
    type: 'SystemAssigned'
  }
  properties: {
    serverFarmId: plan.id
    httpsOnly: true
    siteConfig: {
      linuxFxVersion: 'DOTNETCORE|10.0'
      minTlsVersion: '1.2'
      ftpsState: 'Disabled'
      http20Enabled: true
      alwaysOn: planSku != 'F1'
      healthCheckPath: '/healthz'
      appSettings: [
        { name: 'Demo__Enabled', value: 'true' }
        { name: 'WUTILITY_API_KEY', value: apiKey }
        { name: 'CONNECTION_VAULT_KEY', value: connectionVaultKey }
        // Client IPs for the rate limiter come from X-Forwarded-For.
        { name: 'ASPNETCORE_FORWARDEDHEADERS_ENABLED', value: 'true' }
      ]
      // Surfaced to .NET as ConnectionStrings:<name>. No secret in them: the
      // app signs in as its managed identity. Connect Timeout covers a
      // serverless database resuming from auto-pause.
      connectionStrings: [for d in databases: {
        name: d.setting
        type: 'SQLAzure'
        connectionString: 'Server=tcp:${sqlServer.properties.fullyQualifiedDomainName},1433;Database=${d.name};Authentication=Active Directory Managed Identity;Encrypt=True;Connect Timeout=90;'
      }]
    }
  }
}

output webAppUrl string = 'https://${webApp.properties.defaultHostName}'
output webAppName string = webApp.name
output sqlServerFqdn string = sqlServer.properties.fullyQualifiedDomainName
