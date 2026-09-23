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
