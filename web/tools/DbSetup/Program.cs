using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

// Runs web/db/*.sql against the four demo databases.
//
// Azure (default): signs in as whoever is logged in to the Azure CLI
// ("Active Directory Default"), who must be the SQL server's Entra admin, and
// grants the web app's managed identity its per-database roles:
//   dotnet run --project tools/DbSetup -- --server <name>.database.windows.net --identity <web app name>
//
// Local SQL Server (Windows auth, creates the databases if missing, no grants):
//   dotnet run --project tools/DbSetup -- --server "(local)" --windows [--prefix WuDemoTest_]
//
// sqlcmd can't do Entra sign-in in this environment, and the scripts use
// sqlcmd's $(AppIdentity) variable and GO separators, so this handles both.

var opts = ParseArgs(args);
var server = opts.GetValueOrDefault("server") ?? Fail("--server is required");
var windows = opts.ContainsKey("windows");
var identity = opts.GetValueOrDefault("identity") ?? "";
var prefix = opts.GetValueOrDefault("prefix");
var dbDir = opts.GetValueOrDefault("scripts") ?? FindDbDir();

// Script -> database. With --prefix the names become <prefix>Source etc., so a
// local test run never touches the existing demo databases.
var plan = new (string Script, string Database)[]
{
    ("01-schemasync-source.sql", prefix is null ? "SchemaSyncDemo_Source" : prefix + "Source"),
    ("02-schemasync-target.sql", prefix is null ? "SchemaSyncDemo_Target" : prefix + "Target"),
    ("03-totalsystem.sql",       prefix is null ? "TotalsystemDemo"       : prefix + "Totalsystem"),
    ("04-dbconfig.sql",          prefix is null ? "dbConfigDataBasesDemo" : prefix + "DbConfig"),
};

string ConnectionString(string database) => new SqlConnectionStringBuilder
{
    DataSource = server,
    InitialCatalog = database,
    Encrypt = windows ? SqlConnectionEncryptOption.Optional : SqlConnectionEncryptOption.Mandatory,
    TrustServerCertificate = windows,
    IntegratedSecurity = windows,
    Authentication = windows ? SqlAuthenticationMethod.NotSpecified : SqlAuthenticationMethod.ActiveDirectoryDefault,
    // A free-offer serverless database may be auto-paused; resuming takes up to a minute.
    ConnectTimeout = 90,
}.ConnectionString;

var goSplitter = new Regex(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

foreach (var (script, database) in plan)
{
    if (windows)
    {
        // Azure databases come from infra/main.bicep; locally we create them.
        await using var master = new SqlConnection(ConnectionString("master"));
        await master.OpenAsync();
        await using var create = new SqlCommand(
            "IF DB_ID(@db) IS NULL BEGIN DECLARE @q nvarchar(300) = QUOTENAME(@db); EXEC (N'CREATE DATABASE ' + @q); END", master);
        create.Parameters.AddWithValue("@db", database);
        await create.ExecuteNonQueryAsync();
    }

    var sql = File.ReadAllText(Path.Combine(dbDir, script)).Replace("$(AppIdentity)", identity.Replace("'", "''"));
    await using var conn = new SqlConnection(ConnectionString(database));
    await conn.OpenAsync();
    foreach (var batch in goSplitter.Split(sql).Where(b => !string.IsNullOrWhiteSpace(b)))
    {
        await using var cmd = new SqlCommand(batch, conn) { CommandTimeout = 120 };
        await cmd.ExecuteNonQueryAsync();
    }
    Console.WriteLine($"{script} -> {database}: ok{(identity.Length > 0 ? $" (granted {identity})" : "")}");
}

static Dictionary<string, string?> ParseArgs(string[] args)
{
    var result = new Dictionary<string, string?>();
    for (var i = 0; i < args.Length; i++)
    {
        if (!args[i].StartsWith("--")) continue;
        var key = args[i][2..];
        var value = i + 1 < args.Length && !args[i + 1].StartsWith("--") ? args[++i] : null;
        result[key] = value;
    }
    return result;
}

static string FindDbDir()
{
    for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
    {
        var candidate = Path.Combine(dir.FullName, "db");
        if (File.Exists(Path.Combine(candidate, "01-schemasync-source.sql"))) return candidate;
    }
    return Fail("Couldn't find web/db; pass --scripts <dir>");
}

static string Fail(string message)
{
    Console.Error.WriteLine(message);
    Environment.Exit(1);
    return "";
}
