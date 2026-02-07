using System.Text;
using OrmLib.Attributes;
using OrmLib.Metadata;
using OrmLib.Sql;

namespace OrmLib.Migrations;

/// <summary>
/// Generates migrations by comparing the current database schema with the metadata model.
/// </summary>
public static class MigrationGenerator
{
    /// <summary>
    /// Generates a migration by comparing the current schema with the target metadata.
    /// </summary>
    /// <param name="currentSchema">The current database schema snapshot.</param>
    /// <param name="targetMetadata">The target metadata from entity classes.</param>
    /// <returns>A migration containing the differences, or null if schemas match.</returns>
    public static Migration? GenerateMigration(SchemaSnapshot currentSchema, IEnumerable<TableMetadata> targetMetadata)
    {
        if (currentSchema == null)
            throw new ArgumentNullException(nameof(currentSchema));
        if (targetMetadata == null)
            throw new ArgumentNullException(nameof(targetMetadata));

        var upStatements = new List<string>();
        var downStatements = new List<string>();

        var targetTables = targetMetadata.ToList();

        // Find tables to create (exist in target but not in current)
        foreach (var targetTable in targetTables)
        {
            var currentTable = currentSchema.GetTable(targetTable.TableName);
            if (currentTable == null)
            {
                // Table doesn't exist, create it
                var createTableSql = SqlGenerator.GenerateCreateTable(targetTable);
                upStatements.Add(createTableSql);

                // Down: drop table
                downStatements.Add($"DROP TABLE IF EXISTS \"{targetTable.TableName}\";");
            }
            else
            {
                // Table exists, check for differences
                var (alterUp, alterDown) = CompareTables(currentTable, targetTable);
                upStatements.AddRange(alterUp);
                downStatements.AddRange(alterDown);
            }
        }

        // Find tables to drop (exist in current but not in target)
        foreach (var currentTable in currentSchema.Tables)
        {
            var targetTable = targetTables.FirstOrDefault(t => 
                t.TableName.Equals(currentTable.TableName, StringComparison.OrdinalIgnoreCase));
            
            if (targetTable == null)
            {
                // Table exists in database but not in target metadata, drop it
                downStatements.Add($"DROP TABLE IF EXISTS \"{currentTable.TableName}\";");
                upStatements.Add($"-- Table {currentTable.TableName} removed from model");
            }
        }

        // Filter out comment-only statements (they don't represent actual schema changes)
        var actualUpStatements = upStatements.Where(s => !s.TrimStart().StartsWith("--")).ToList();
        var actualDownStatements = downStatements.Where(s => !s.TrimStart().StartsWith("--")).ToList();

        if (actualUpStatements.Count == 0 && actualDownStatements.Count == 0)
        {
            return null; // No differences
        }

        var migrationId = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var migrationName = $"Migration_{migrationId}";

        return new Migration(migrationId, migrationName, actualUpStatements, actualDownStatements);
    }

    private static (List<string> UpStatements, List<string> DownStatements) CompareTables(
        TableSnapshot currentTable,
        TableMetadata targetTable)
    {
        var upStatements = new List<string>();
        var downStatements = new List<string>();

        // Compare columns
        var currentColumns = currentTable.Columns.ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);
        var targetColumns = targetTable.Columns.ToDictionary(c => c.ColumnName, StringComparer.OrdinalIgnoreCase);

        // Find columns to add
        foreach (var targetColumn in targetTable.Columns)
        {
            if (!currentColumns.ContainsKey(targetColumn.ColumnName))
            {
                // Column doesn't exist, add it
                var addColumnSql = GenerateAddColumn(targetTable, targetColumn);
                upStatements.Add(addColumnSql);

                // Down: drop column
                downStatements.Add($"ALTER TABLE \"{targetTable.TableName}\" DROP COLUMN IF EXISTS \"{targetColumn.ColumnName}\";");
            }
            else
            {
                // Column exists, check for differences
                var currentColumn = currentColumns[targetColumn.ColumnName];
                var (alterUp, alterDown) = CompareColumns(currentTable.TableName, currentColumn, targetColumn);
                if (alterUp != null)
                {
                    upStatements.Add(alterUp);
                    if (alterDown != null)
                    {
                        downStatements.Add(alterDown);
                    }
                }
            }
        }

        // Find columns to drop
        foreach (var currentColumn in currentTable.Columns)
        {
            if (!targetColumns.ContainsKey(currentColumn.ColumnName))
            {
                // Column exists in database but not in target, drop it
                downStatements.Add($"ALTER TABLE \"{currentTable.TableName}\" DROP COLUMN IF EXISTS \"{currentColumn.ColumnName}\";");
                upStatements.Add($"-- Column {currentColumn.ColumnName} removed from model");
            }
        }

        return (upStatements, downStatements);
    }

    private static string GenerateAddColumn(TableMetadata table, ColumnMetadata column)
    {
        var sql = new StringBuilder();
        sql.Append($"ALTER TABLE \"{table.TableName}\" ADD COLUMN ");
        sql.Append($"\"{column.ColumnName}\" ");
        sql.Append(GetSqlDataType(column));
        
        if (!column.IsRequired)
        {
            sql.Append(" NULL");
        }
        else
        {
            sql.Append(" NOT NULL");
        }

        if (column.IsUnique)
        {
            sql.Append($" UNIQUE");
        }

        if (column.DefaultValue != null)
        {
            if (column.IsDefaultSqlExpression)
            {
                sql.Append($" DEFAULT {column.DefaultValue}");
            }
            else
            {
                sql.Append($" DEFAULT '{column.DefaultValue}'");
            }
        }

        sql.Append(";");
        return sql.ToString();
    }

    private static (string? UpStatement, string? DownStatement) CompareColumns(
        string tableName,
        ColumnSnapshot currentColumn,
        ColumnMetadata targetColumn)
    {
        var upStatements = new List<string>();
        var downStatements = new List<string>();

        // Compare data type (normalize to base type for comparison)
        var currentDataType = NormalizeDataType(currentColumn.DataType);
        var targetDataType = GetSqlDataType(targetColumn);
        var normalizedTargetDataType = NormalizeDataType(targetDataType);

        // Only report data type changes if the base type actually changed
        // (ignore length/precision differences for VARCHAR, CHAR, DECIMAL as they can be altered separately if needed)
        if (currentDataType != normalizedTargetDataType)
        {
            // Data type changed - but check if it's just a length/precision difference
            var currentBaseType = currentDataType.Split('(')[0].Trim();
            var targetBaseType = normalizedTargetDataType.Split('(')[0].Trim();
            
            if (currentBaseType != targetBaseType)
            {
                // Base type changed, this is a real change
                upStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" TYPE {targetDataType};");
                downStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" TYPE {currentColumn.DataType};");
            }
            // If only length/precision differs, we skip it (PostgreSQL can handle this automatically)
        }

        // Compare nullable
        if (currentColumn.IsNullable != !targetColumn.IsRequired)
        {
            if (targetColumn.IsRequired)
            {
                upStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" SET NOT NULL;");
                downStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" DROP NOT NULL;");
            }
            else
            {
                upStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" DROP NOT NULL;");
                downStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" SET NOT NULL;");
            }
        }

        // Compare default value (skip for auto-increment primary keys as they use sequences)
        if (!(targetColumn.IsPrimaryKey && targetColumn.IsAutoIncrement))
        {
            var currentDefault = NormalizeDefaultValue(currentColumn.DefaultValue);
            var targetDefault = NormalizeDefaultValue(targetColumn.DefaultValue);
            if (currentDefault != targetDefault)
            {
                if (targetColumn.DefaultValue != null)
                {
                    if (targetColumn.IsDefaultSqlExpression)
                    {
                        upStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" SET DEFAULT {targetColumn.DefaultValue};");
                    }
                    else
                    {
                        upStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" SET DEFAULT '{targetColumn.DefaultValue}';");
                    }
                }
                else
                {
                    upStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" DROP DEFAULT;");
                }

                if (currentColumn.DefaultValue != null)
                {
                    downStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" SET DEFAULT {currentColumn.DefaultValue};");
                }
                else
                {
                    downStatements.Add($"ALTER TABLE \"{tableName}\" ALTER COLUMN \"{targetColumn.ColumnName}\" DROP DEFAULT;");
                }
            }
        }

        // Note: UNIQUE constraints are not compared here as they require DROP/ADD CONSTRAINT operations
        // which are more complex. For now, we only compare column-level properties.

        if (upStatements.Count == 0)
            return (null, null);

        return (string.Join(" ", upStatements), string.Join(" ", downStatements));
    }

    private static string NormalizeDataType(string dataType)
    {
        // Normalize data type for comparison
        var normalized = dataType.ToUpper().Trim();
        
        // Remove length/precision/scale for comparison (e.g., VARCHAR(50) -> VARCHAR)
        var baseType = normalized.Split('(')[0].Trim();
        
        // BIGSERIAL is stored as BIGINT in information_schema, but we generate BIGSERIAL
        // Treat them as equivalent for comparison purposes
        if (baseType == "BIGINT" || baseType == "BIGSERIAL")
        {
            return "BIGINT"; // Normalize both to BIGINT for comparison
        }
        
        // INT and INTEGER are equivalent
        if (baseType == "INT" || baseType == "INTEGER")
        {
            return "INT";
        }
        
        return baseType;
    }

    private static string? NormalizeDefaultValue(string? defaultValue)
    {
        if (defaultValue == null)
            return null;

        // Remove quotes and normalize
        return defaultValue.Trim('\'', '"');
    }

    private static string GetSqlDataType(ColumnMetadata column)
    {
        if (!column.DataType.HasValue)
        {
            throw new InvalidOperationException($"Column {column.ColumnName} does not have a data type specified.");
        }

        return column.DataType.Value switch
        {
            SqlDataType.Int => column.IsAutoIncrement && column.IsPrimaryKey
                ? "BIGSERIAL"  // Use BIGSERIAL for auto-increment primary keys
                : "INT",
            SqlDataType.Decimal => column.Precision.HasValue && column.Scale.HasValue
                ? $"DECIMAL({column.Precision.Value},{column.Scale.Value})"
                : column.Precision.HasValue
                    ? $"DECIMAL({column.Precision.Value})"
                    : "DECIMAL",
            SqlDataType.Float => "FLOAT",
            SqlDataType.Varchar => column.Length.HasValue ? $"VARCHAR({column.Length.Value})" : "VARCHAR",
            SqlDataType.Char => column.Length.HasValue ? $"CHAR({column.Length.Value})" : "CHAR(1)",
            SqlDataType.Text => "TEXT",
            SqlDataType.TimestampWithTimeZone => "TIMESTAMP WITH TIMEZONE",
            SqlDataType.TimestampWithoutTimeZone => "TIMESTAMP WITHOUT TIMEZONE",
            _ => throw new NotSupportedException($"Unsupported SQL data type: {column.DataType.Value}")
        };
    }
}

