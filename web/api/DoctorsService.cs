using Microsoft.Data.SqlClient;

namespace SchemaSyncApi;

// Web equivalent of DoctorsForm.xaml.cs + DoctorsPriceGroupsForm.xaml.cs's
// data access — same queries (see FillDoctorsGrid / FillDoctorPriceGroupsGrid
// in the original WPF code), reimplemented against Microsoft.Data.SqlClient
// instead of the WPF app's own DBHelper.
public class DoctorsService
{
    private readonly string _connectionString;

    public DoctorsService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Totalsystem")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:Totalsystem");
    }

    public async Task<List<Doctor>> GetDoctorsAsync()
    {
        const string sql = "SELECT Code, fName, lName, Name, PostNo FROM dbo.Doctors ORDER BY Name";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ToListAsync(r => new Doctor(
            r.GetInt32(0), r.GetString(1), r.GetString(2),
            r.GetString(3), r.IsDBNull(4) ? null : r.GetString(4)));
    }

    public async Task<List<Work>> GetWorksAsync()
    {
        const string sql = "SELECT Code, Name FROM dbo.Works ORDER BY Name";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ToListAsync(r => new Work(r.GetInt32(0), r.GetString(1)));
    }

    public async Task<List<DoctorPriceGroup>> GetPriceGroupsAsync(int? drCode)
    {
        var sql = @"
            SELECT Dp.RowGD, Dp.DrCode, Dp.WCode, Dp.CenterSharePrice, Dp.Title, D.Name AS DrName, W.Name AS WName
            FROM dbo.DoctorsPriceGroups Dp
            INNER JOIN dbo.Doctors D ON D.Code = Dp.DrCode
            LEFT JOIN dbo.Works W ON W.Code = Dp.WCode";
        if (drCode is not null) sql += " WHERE Dp.DrCode = @DrCode";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        if (drCode is not null) cmd.Parameters.AddWithValue("@DrCode", drCode.Value);
        await using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ToListAsync(r => new DoctorPriceGroup(
            r.GetInt32(0), r.GetInt32(1),
            r.IsDBNull(2) ? null : r.GetInt32(2),
            r.IsDBNull(3) ? null : r.GetDecimal(3),
            r.IsDBNull(4) ? null : r.GetString(4),
            r.IsDBNull(5) ? null : r.GetString(5),
            r.IsDBNull(6) ? null : r.GetString(6)
        ));
    }

    public async Task<int> AddPriceGroupAsync(UpsertDoctorPriceGroupRequest req)
    {
        const string sql = @"
            INSERT INTO dbo.DoctorsPriceGroups (DrCode, WCode, CenterSharePrice, Title)
            OUTPUT INSERTED.RowGD
            VALUES (@DrCode, @WCode, @CenterSharePrice, @Title)";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        AddUpsertParams(cmd, req);
        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task UpdatePriceGroupAsync(int rowGd, UpsertDoctorPriceGroupRequest req)
    {
        const string sql = @"
            UPDATE dbo.DoctorsPriceGroups
            SET DrCode = @DrCode, WCode = @WCode, CenterSharePrice = @CenterSharePrice, Title = @Title
            WHERE RowGD = @RowGD";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        AddUpsertParams(cmd, req);
        cmd.Parameters.AddWithValue("@RowGD", rowGd);
        await cmd.ExecuteNonQueryAsync();
    }

    public async Task DeletePriceGroupAsync(int rowGd)
    {
        const string sql = "DELETE FROM dbo.DoctorsPriceGroups WHERE RowGD = @RowGD";
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@RowGD", rowGd);
        await cmd.ExecuteNonQueryAsync();
    }

    private static void AddUpsertParams(SqlCommand cmd, UpsertDoctorPriceGroupRequest req)
    {
        cmd.Parameters.AddWithValue("@DrCode", req.DrCode);
        cmd.Parameters.AddWithValue("@WCode", (object?)req.WCode ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@CenterSharePrice", (object?)req.CenterSharePrice ?? DBNull.Value);
        cmd.Parameters.AddWithValue("@Title", (object?)req.Title ?? DBNull.Value);
    }
}
