using Microsoft.Data.SqlClient;

namespace SchemaSyncApi;

// Web equivalent of Connections.xaml.cs — but a real server-side vault
// (one SiteConnections table, AES-GCM encrypted values) instead of the
// original's per-module local app-settings slots. The decrypted connection
// string never leaves this service: list/test endpoints only ever return
// success/failure or metadata, never the plaintext.
public class SiteConnectionsService
{
    private readonly string _connectionString;
    private readonly ConnectionVault _vault;

    public SiteConnectionsService(IConfiguration configuration, ConnectionVault vault)
    {
        _connectionString = configuration.GetConnectionString("DbConfig")
            ?? throw new InvalidOperationException("Missing ConnectionStrings:DbConfig");
        _vault = vault;
    }

    public async Task<List<SiteConnectionSummary>> ListAsync(string? moduleName)
    {
        var sql = "SELECT Id, ModuleName, DisplayName, CreatedAt FROM dbo.SiteConnections";
        if (moduleName is not null) sql += " WHERE ModuleName = @ModuleName";
        sql += " ORDER BY DisplayName";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        if (moduleName is not null) cmd.Parameters.AddWithValue("@ModuleName", moduleName);
        await using var reader = await cmd.ExecuteReaderAsync();

        return await reader.ToListAsync(r => new SiteConnectionSummary(
            r.GetInt32(0), r.GetString(1), r.GetString(2), r.GetDateTime(3)));
    }

    public async Task<int> AddAsync(AddSiteConnectionRequest req)
    {
        var encrypted = _vault.Encrypt(req.ConnectionString);

        const string sql = @"
            INSERT INTO dbo.SiteConnections (ModuleName, DisplayName, EncryptedConnectionString)
            OUTPUT INSERTED.Id
            VALUES (@ModuleName, @DisplayName, @Encrypted)";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@ModuleName", req.ModuleName);
        cmd.Parameters.AddWithValue("@DisplayName", req.DisplayName);
        cmd.Parameters.AddWithValue("@Encrypted", encrypted);

        var result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt32(result);
    }

    public async Task<TestConnectionResult> TestAsync(int id)
    {
        var encrypted = await GetEncryptedAsync(id);
        if (encrypted is null) return new TestConnectionResult(false, "Connection not found.");

        string plaintext;
        try
        {
            plaintext = _vault.Decrypt(encrypted);
        }
        catch (Exception)
        {
            // Almost always means the process restarted since this row was
            // encrypted with a different ephemeral key — see ConnectionVault's
            // own comment. Not the target database's fault.
            return new TestConnectionResult(false, "Could not decrypt this connection (vault key changed since it was saved).");
        }

        try
        {
            await using var testConn = new SqlConnection(plaintext);
            await testConn.OpenAsync();
            return new TestConnectionResult(true, null);
        }
        catch (Exception ex)
        {
            return new TestConnectionResult(false, ex.Message);
        }
    }

    public async Task DeleteAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand("DELETE FROM dbo.SiteConnections WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        await cmd.ExecuteNonQueryAsync();
    }

    private async Task<string?> GetEncryptedAsync(int id)
    {
        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();
        await using var cmd = new SqlCommand(
            "SELECT EncryptedConnectionString FROM dbo.SiteConnections WHERE Id = @Id", conn);
        cmd.Parameters.AddWithValue("@Id", id);
        var result = await cmd.ExecuteScalarAsync();
        return result as string;
    }
}
