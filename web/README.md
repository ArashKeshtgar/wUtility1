# wUtility Web

A web port of wUtility's core features — same problem (SQL Server schema comparison/sync for hospital deployment sites, plus the doctors/price-groups/site-registry config forms), reimplemented as a .NET minimal API + Angular frontend instead of the original WPF/DevExpress/Telerik desktop app. The original WPF app's code is untouched; this lives alongside it as an independent implementation against the same table shapes.

## Why a web port

The desktop app's UI logic (DevExpress WPF grids, Telerik controls) can't run in a browser, and its licensed component dependencies aren't in this repo. Rather than trying to translate WPF XAML directly, each form's actual data behavior (queries, joins, CRUD operations) was read from the original `.xaml.cs` files and reimplemented cleanly against the same tables.

## What's ported so far

| Original WPF form | Web equivalent | Notes |
|---|---|---|
| `MainWindow` | Top nav bar (Angular) | Menu shell only |
| `SimilarityDatabaseForm` | `/schema-sync` page | Schema diff + generated T-SQL sync script, review-then-apply |
| `DoctorsForm` | `/doctors` page | Read-only list from `Doctors` |
| `DoctorsPriceGroupsForm` | `/doctor-price-groups` page | Full CRUD on `DoctorsPriceGroups`, joined with `Doctors`/`Works` |
| `DatabasesForm` (top grid) | `/db-config` page | Read-only list from `Tbl_dbConfigSetting` |

`DatabasesForm`'s "connected databases per module" grid and `Connections.xaml.cs` are not a database feature in the original — they're per-module (13 modules) AES-encrypted connection strings stored in the local machine's app settings, with a hardcoded encryption key. That doesn't have a safe or meaningful equivalent in a shared web app; a proper server-side connection vault is being built separately rather than copying that pattern.

## Tech stack

- Backend: ASP.NET Core minimal API (.NET 10), `Microsoft.Data.SqlClient`
- Frontend: Angular 18, standalone components, `provideRouter`
- All schema/CRUD logic reads real SQL Server metadata and tables — no mocked data layer

## LINQ and lambda usage

- `SchemaDiffer.cs` — `ToDictionary`, `Select`, `Where`, `FirstOrDefault` to compare two schema snapshots
- `ScriptGenerator.cs` — `Select(...).Distinct()`, `Where`, `First`, `OrderBy` to turn a diff list into a T-SQL script
- `SqlReaderExtensions.cs` — a generic `ToListAsync<T>(this SqlDataReader, Func<SqlDataReader, T> map)` extension method, replacing repeated `while (reader.Read())` loops across `DoctorsService`, `DbConfigService`, and `SchemaReader` with a single lambda-driven mapper
- `SchemaReader.cs` — reads flat metadata rows, then `GroupBy(row => row.TableName)` to rebuild the table/column hierarchy, instead of manual dictionary bookkeeping

## Testing performed

All of the above were tested against real SQL Server data on `(local)` (dedicated demo databases — `SchemaSyncDemo_Source/Target`, `TotalsystemDemo`, `dbConfigDataBasesDemo` — never the real hospital databases on the same instance):

- Schema compare correctly detected a missing table, a missing column, and a column type mismatch; the generated script was executed and a re-compare confirmed zero remaining diffs.
- Doctors → price-groups navigation, add, and edit were exercised end-to-end through the actual browser UI, with real inserts/updates verified in the database.

## Running it (API key required)

Every `/api/*` route requires an `X-Api-Key` header. Without it, `/api/execute` (which runs a script against a target server) would be open to anyone who can reach the port. The API refuses to start unless `WUTILITY_API_KEY` is set to a random value of at least 32 characters. There is no default.

```bash
# PowerShell: $env:WUTILITY_API_KEY = "<random 32+ chars>"
cd api && dotnet run                  # :5091
cd frontend && npm start              # :4300, same WUTILITY_API_KEY in the environment
```

The Angular app calls a relative `/api`. In development, `ng serve` proxies it to the API through `frontend/proxy.conf.js`, which adds the key on the server side, so the key never ends up in the browser bundle. Control Panel's `wutility-adapter` sends the same header. This is service-to-service protection, not user authentication: a real deployment would still need user login in front of the schema-sync pages.


## Public demo on Azure

`infra/main.bicep` + `deploy/azure/deploy.ps1` deploy a public demo: one Linux App Service (.NET 10) serving both the API and the Angular build from the same origin, and the four demo databases on the Azure SQL Database free offer (serverless, auto-pause, pauses rather than bills when the monthly free budget runs out).

- **No SQL passwords anywhere.** The SQL server is Microsoft Entra-only; the web app signs in with its system-assigned managed identity (`Authentication=Active Directory Managed Identity`, which in SqlClient 7 needs the `Microsoft.Data.SqlClient.Extensions.Azure` package). Each database grants that identity only what it needs (`db/*.sql`): read-only on the source, DDL on the sync target, read/write on the two config databases.
- **Demo mode (`Demo__Enabled=true`, `api/DemoMode.cs`).** The browser can't hold the API key, so instead of trusting it the risky routes are constrained: compare/execute always use the two sandbox databases and ignore client connection strings; execute only runs the exact script the server generates from the current diff (anything else → 409); the connection vault is read-only (adding/testing a connection string would make the host connect wherever it points); `POST /api/demo/reset` restores the drifted target schema and price-group rows. Per-IP rate limits protect the free vCore budget. Outside demo mode nothing changes: every `/api` route still needs `X-Api-Key`.
- **Schemas are in the repo.** `db/01..04-*.sql` recreate each demo database (idempotent). `tools/DbSetup` runs them with Entra sign-in on Azure, or with Windows auth locally: `dotnet run --project tools/DbSetup -- --server "(local)" --windows --prefix WuDemoTest_`.

```powershell
az login
./deploy/azure/deploy.ps1 -AppName <globally-unique-name> -WhatIf   # preview
./deploy/azure/deploy.ps1 -AppName <globally-unique-name>           # deploy
./deploy/azure/deploy.ps1 -AppName <globally-unique-name> -CodeOnly # redeploy the app only
```

The first request after the databases have been idle can take up to a minute while serverless SQL resumes.
