using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;

namespace SchemaSyncApi;

// Runs a generated script batch-by-batch (splitting on GO, same convention
// sqlcmd/SSMS use) so one failing statement doesn't silently swallow the rest
// — the caller gets back exactly which statements ran before a failure.
public class ScriptExecutor
{
    private static readonly Regex GoSplitter = new(@"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);

    // A batch is executable if it has at least one non-empty, non-comment line —
    // a batch made up entirely of "-- ..." lines (like the script's header) is
    // just documentation and should be skipped, not sent to SQL Server as-is.
    private static bool HasExecutableContent(string batch) =>
        batch.Split('\n').Any(line => line.Trim() is { Length: > 0 } t && !t.StartsWith("--"));

    public async Task<ExecuteResult> ExecuteAsync(string connectionString, string script)
    {
        var batches = GoSplitter.Split(script)
            .Select(b => b.Trim())
            .Where(HasExecutableContent)
            .ToList();

        var executed = new List<string>();

        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        foreach (var batch in batches)
        {
            try
            {
                await using var cmd = new SqlCommand(batch, conn);
                await cmd.ExecuteNonQueryAsync();
                executed.Add(batch);
            }
            catch (Exception ex)
            {
                return new ExecuteResult(false, executed, $"Failed on batch:\n{batch}\n\nError: {ex.Message}");
            }
        }

        return new ExecuteResult(true, executed, null);
    }
}
