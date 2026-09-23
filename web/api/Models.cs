namespace SchemaSyncApi;

public record ColumnInfo(
    string Name,
    string DataType,
    int? MaxLength,
    int? Precision,
    int? Scale,
    bool IsNullable,
    bool IsIdentity,
    int OrdinalPosition
)
{
    // The SQL type fragment usable in CREATE/ALTER, e.g. "nvarchar(20)", "decimal(10,2)", "int".
    public string SqlTypeExpression()
    {
        var needsLength = DataType is "varchar" or "nvarchar" or "char" or "nchar" or "varbinary";
        var needsPrecisionScale = DataType is "decimal" or "numeric";

        if (needsLength && MaxLength is not null)
        {
            // INFORMATION_SCHEMA.COLUMNS.CHARACTER_MAXIMUM_LENGTH is already in
            // characters for nvarchar/nchar (SQL Server reports it pre-converted),
            // so no /2 adjustment is needed here.
            var len = MaxLength == -1 ? "MAX" : MaxLength.ToString();
            return $"{DataType}({len})";
        }
        if (needsPrecisionScale && Precision is not null)
        {
            return $"{DataType}({Precision},{Scale})";
        }
        return DataType;
    }
}

public record TableInfo(string Name, List<ColumnInfo> Columns);

public record SchemaSnapshot(List<TableInfo> Tables);

public enum DiffKind
{
    MissingTable,
    MissingColumn,
    ColumnTypeMismatch,
    ColumnNullabilityMismatch
}

public record DiffEntry(DiffKind Kind, string TableName, string? ColumnName, string Description);

public record CompareRequest(string SourceConnectionString, string TargetConnectionString);

public record CompareResponse(List<DiffEntry> Diffs, string Script);

public record ExecuteRequest(string TargetConnectionString, string Script);

public record ExecuteResult(bool Success, List<string> ExecutedStatements, string? Error);
