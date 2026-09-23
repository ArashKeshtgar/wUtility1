using Microsoft.Data.SqlClient;

namespace SchemaSyncApi;

public record DbConfigSetting(
    int FldId, string FldMainDataBase, string FldDataBaseName, string? FldDelphiConnectionName,
    string? FldPersianDescDataBase, bool FldIsActive, string? FldInstanceName, string? FldSecondInstance
);

// Web port of DatabasesForm.xaml.cs's top grid (Tbl_dbConfigSetting) — see
// Models/Tbl_dbConfigSetting.cs in the WPF app. Read-only there and read-only
// here: the original app never writes to this table either.
public class DbConfigService
{
    private readonly string _connectionString;

    public DbConfigService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("DbConfig")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:DbConfig");
    }

    public async Task<List<DbConfigSetting>> GetSettingsAsync()
    {
        const string sql = @"
            SELECT fldID, fldMainDataBase, fldDataBaseName, fldDelphiConnectionName,
                   fldPersianDescDataBase, fldIsActive, fldInstanceName, fldSecondInstance
            FROM dbo.Tbl_dbConfigSetting
            ORDER BY fldID";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ToListAsync(r => new DbConfigSetting(
            r.GetInt32(0), r.GetString(1), r.GetString(2),
            r.IsDBNull(3) ? null : r.GetString(3),
            r.IsDBNull(4) ? null : r.GetString(4),
            r.GetBoolean(5),
            r.IsDBNull(6) ? null : r.GetString(6),
            r.IsDBNull(7) ? null : r.GetString(7)
        ));
    }
}
