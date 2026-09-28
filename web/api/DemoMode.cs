using Microsoft.Data.SqlClient;

namespace SchemaSyncApi;

// Public-demo mode (Demo:Enabled=true), used for the Azure deployment.
//
// Outside demo mode the API is a service-to-service tool behind X-Api-Key:
// /api/compare and /api/execute take any connection string and run any
// script, which is the point of the tool. On the public internet that would
// be an open SQL runner and network probe, so demo mode keeps the pages but
// takes those powers away instead of trusting a browser-held key:
//   - compare/execute always use the two configured demo databases
//     (ConnectionStrings:SchemaSyncSource / SchemaSyncTarget); client-sent
//     connection strings are ignored;
//   - execute only runs the script the server itself generates from the
//     current diff, and refuses if the client's copy differs from it, so no
//     caller-written SQL ever reaches the database;
//   - the connection vault is read-only (adding/testing a connection string
//     would let anyone make this host open connections to arbitrary servers);
//   - /api/demo/reset puts the target schema and the price-group rows back to
//     their starting state, so visitors can repeat the compare -> apply flow.
public class DemoOptions
{
    public bool Enabled { get; set; }
}

public record DemoInfo(bool Enabled, string? SourceDatabase, string? TargetDatabase);

public class DemoDatabases
{
    public string Source { get; }
    public string Target { get; }
    public string SourceName { get; }
    public string TargetName { get; }

    public DemoDatabases(IConfiguration configuration)
    {
        Source = configuration.GetConnectionString("SchemaSyncSource")
            ?? throw new InvalidOperationException("Demo mode needs ConnectionStrings:SchemaSyncSource");
        Target = configuration.GetConnectionString("SchemaSyncTarget")
            ?? throw new InvalidOperationException("Demo mode needs ConnectionStrings:SchemaSyncTarget");
        SourceName = new SqlConnectionStringBuilder(Source).InitialCatalog;
        TargetName = new SqlConnectionStringBuilder(Target).InitialCatalog;
    }
}

public class DemoResetService
{
    // The target's starting "drift": no Appointments table, Patients.Phone
    // missing, NationalCode too short, BirthDate NOT NULL. One of each diff
    // kind SchemaDiffer reports. Same statements as db/02-schemasync-target.sql.
    private const string ResetTargetSql = @"
        IF OBJECT_ID('dbo.Appointments') IS NOT NULL DROP TABLE dbo.Appointments;
        IF OBJECT_ID('dbo.Patients') IS NOT NULL DROP TABLE dbo.Patients;
        CREATE TABLE dbo.Patients (
            PatientId    int IDENTITY(1,1) NOT NULL PRIMARY KEY,
            FullName     nvarchar(150) NOT NULL,
            NationalCode nvarchar(10)  NOT NULL,
            BirthDate    date          NOT NULL
        );";

    // Same rows as db/03-totalsystem.sql. RowGD keeps counting up across
    // resets; resetting the identity would need ALTER permission, which the
    // app's database user deliberately doesn't have.
    private const string ResetPriceGroupsSql = @"
        DELETE FROM dbo.DoctorsPriceGroups;
        INSERT INTO dbo.DoctorsPriceGroups (DrCode, WCode, CenterSharePrice, Title) VALUES
            (101, 1, 350000.00,  N'ویزیت عمومی'),
            (101, 2, 1200000.00, N'جراحی سرپایی'),
            (102, 3, 500000.00,  N'سونوگرافی شکم'),
            (101, 3, 750000.00,  N'سونوگرافی تخصصی');";

    private readonly DemoDatabases _databases;
    private readonly string _totalsystem;

    public DemoResetService(DemoDatabases databases, IConfiguration configuration)
    {
        _databases = databases;
        _totalsystem = configuration.GetConnectionString("Totalsystem")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:Totalsystem");
    }

    public async Task ResetAsync()
    {
        await RunAsync(_databases.Target, ResetTargetSql);
        await RunAsync(_totalsystem, ResetPriceGroupsSql);
    }

    private static async Task RunAsync(string connectionString, string sql)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await cmd.ExecuteNonQueryAsync();
    }
}
