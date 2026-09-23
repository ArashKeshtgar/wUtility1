namespace SchemaSyncApi;

// Compares two snapshots: source is the desired state, target is what's there
// now. Diffs are one-directional (source -> target), matching how a hospital
// deployment actually needs it: "bring this site's DB in line with the
// reference schema."
public class SchemaDiffer
{
    public List<DiffEntry> Diff(SchemaSnapshot source, SchemaSnapshot target)
    {
        var diffs = new List<DiffEntry>();
        var targetTables = target.Tables.ToDictionary(t => t.Name, StringComparer.OrdinalIgnoreCase);

        foreach (var sourceTable in source.Tables)
        {
            if (!targetTables.TryGetValue(sourceTable.Name, out var targetTable))
            {
                diffs.Add(new DiffEntry(
                    DiffKind.MissingTable, sourceTable.Name, null,
                    $"Table '{sourceTable.Name}' exists in source but not in target."));
                continue;
            }

            var targetColumns = targetTable.Columns.ToDictionary(c => c.Name, StringComparer.OrdinalIgnoreCase);

            foreach (var sourceColumn in sourceTable.Columns)
            {
                if (!targetColumns.TryGetValue(sourceColumn.Name, out var targetColumn))
                {
                    diffs.Add(new DiffEntry(
                        DiffKind.MissingColumn, sourceTable.Name, sourceColumn.Name,
                        $"Column '{sourceTable.Name}.{sourceColumn.Name}' exists in source but not in target."));
                    continue;
                }

                if (!string.Equals(targetColumn.SqlTypeExpression(), sourceColumn.SqlTypeExpression(), StringComparison.OrdinalIgnoreCase))
                {
                    diffs.Add(new DiffEntry(
                        DiffKind.ColumnTypeMismatch, sourceTable.Name, sourceColumn.Name,
                        $"Column '{sourceTable.Name}.{sourceColumn.Name}' is {targetColumn.SqlTypeExpression()} in target, " +
                        $"{sourceColumn.SqlTypeExpression()} in source."));
                }
                else if (targetColumn.IsNullable != sourceColumn.IsNullable)
                {
                    diffs.Add(new DiffEntry(
                        DiffKind.ColumnNullabilityMismatch, sourceTable.Name, sourceColumn.Name,
                        $"Column '{sourceTable.Name}.{sourceColumn.Name}' nullability differs " +
                        $"(target: {(targetColumn.IsNullable ? "NULL" : "NOT NULL")}, source: {(sourceColumn.IsNullable ? "NULL" : "NOT NULL")})."));
                }
            }
        }

        return diffs;
    }
}
