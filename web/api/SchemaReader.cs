using Microsoft.Data.SqlClient;

namespace SchemaSyncApi;

// Reads a database's table/column structure via SQL Server's own metadata views
// (INFORMATION_SCHEMA + sys.identity_columns) — the same "drive it from metadata,
// not hand-written diff scripts" idea the WPF wUtility tool uses, reimplemented
// independently here for the web API.
public class SchemaReader
{
    private record FlatColumnRow(string TableName, ColumnInfo Column);

    public async Task<SchemaSnapshot> ReadAsync(string connectionString)
    {
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        const string sql = @"
            SELECT
                c.TABLE_NAME,
                c.COLUMN_NAME,
                c.DATA_TYPE,
                c.CHARACTER_MAXIMUM_LENGTH,
                c.NUMERIC_PRECISION,
                c.NUMERIC_SCALE,
                c.IS_NULLABLE,
                c.ORDINAL_POSITION,
                CASE WHEN ic.column_id IS NOT NULL THEN 1 ELSE 0 END AS IsIdentity
            FROM INFORMATION_SCHEMA.COLUMNS c
            JOIN INFORMATION_SCHEMA.TABLES t
                ON t.TABLE_NAME = c.TABLE_NAME AND t.TABLE_SCHEMA = c.TABLE_SCHEMA
            LEFT JOIN sys.identity_columns ic
                ON ic.object_id = OBJECT_ID(QUOTENAME(c.TABLE_SCHEMA) + '.' + QUOTENAME(c.TABLE_NAME))
               AND ic.name = c.COLUMN_NAME
            WHERE t.TABLE_TYPE = 'BASE TABLE' AND c.TABLE_SCHEMA = 'dbo'
            ORDER BY c.TABLE_NAME, c.ORDINAL_POSITION";

        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        var flatRows = await reader.ToListAsync(r => new FlatColumnRow(
            r.GetString(0),
            new ColumnInfo(
                Name: r.GetString(1),
                DataType: r.GetString(2),
                MaxLength: r.IsDBNull(3) ? null : Convert.ToInt32(r.GetValue(3)),
                Precision: r.IsDBNull(4) ? null : Convert.ToInt32(r.GetValue(4)),
                Scale: r.IsDBNull(5) ? null : Convert.ToInt32(r.GetValue(5)),
                IsNullable: r.GetString(6) == "YES",
                IsIdentity: r.GetInt32(8) == 1,
                OrdinalPosition: r.GetInt32(7)
            )));

        // GROUP BY the flat metadata rows into one TableInfo per table, in the
        // order tables were first seen (the query's own ORDER BY already keeps
        // each table's columns in ordinal order within its group).
        var tables = flatRows
            .GroupBy(row => row.TableName)
            .Select(g => new TableInfo(g.Key, g.Select(row => row.Column).ToList()));

        return new SchemaSnapshot(tables.ToList());
    }
}
