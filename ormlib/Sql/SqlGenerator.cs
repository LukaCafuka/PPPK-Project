using System.Text;
using OrmLib.Attributes;
using OrmLib.Metadata;

namespace OrmLib.Sql;

/// <summary>
/// Generates PostgreSQL DDL SQL statements.
/// </summary>
public static class SqlGenerator
{
    /// <summary>
    /// Generates a CREATE TABLE statement for the specified table metadata.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <returns>The CREATE TABLE SQL statement.</returns>
    public static string GenerateCreateTable(TableMetadata tableMetadata)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        var sql = new StringBuilder();
        sql.Append($"CREATE TABLE \"{tableMetadata.TableName}\" (");
        sql.AppendLine();

        var columnDefinitions = new List<string>();

        foreach (var column in tableMetadata.Columns)
        {
            var columnDef = GenerateColumnDefinition(column);
            columnDefinitions.Add($"    {columnDef}");
        }

        sql.AppendLine(string.Join(",\n", columnDefinitions));
        sql.Append(")");

        return sql.ToString();
    }

    /// <summary>
    /// Generates a DROP TABLE statement for the specified table metadata.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <param name="ifExists">If true, adds IF EXISTS clause.</param>
    /// <returns>The DROP TABLE SQL statement.</returns>
    public static string GenerateDropTable(TableMetadata tableMetadata, bool ifExists = true)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        var sql = new StringBuilder();
        sql.Append("DROP TABLE");

        if (ifExists)
            sql.Append(" IF EXISTS");

        sql.Append($" \"{tableMetadata.TableName}\"");

        return sql.ToString();
    }

    private static string GenerateColumnDefinition(ColumnMetadata column)
    {
        var sb = new StringBuilder();
        sb.Append($"\"{column.ColumnName}\" ");

        // Generate data type
        sb.Append(GenerateDataType(column));

        // Add constraints
        var constraints = new List<string>();

        if (column.IsPrimaryKey)
        {
            // Add PRIMARY KEY constraint (needed even for BIGSERIAL)
            constraints.Add("PRIMARY KEY");
        }

        if (column.IsRequired && !column.IsPrimaryKey)
        {
            constraints.Add("NOT NULL");
        }

        if (column.IsUnique)
        {
            constraints.Add("UNIQUE");
        }

        if (column.DefaultValue != null)
        {
            if (column.IsDefaultSqlExpression)
            {
                constraints.Add($"DEFAULT {column.DefaultValue}");
            }
            else
            {
                // Escape and quote the default value
                var defaultValue = EscapeDefaultValue(column.DefaultValue, column.DataType);
                constraints.Add($"DEFAULT {defaultValue}");
            }
        }

        if (column.ForeignKeyMetadata != null &&
            !string.IsNullOrEmpty(column.ForeignKeyMetadata.ReferencedTableName) &&
            !string.IsNullOrEmpty(column.ForeignKeyMetadata.ReferencedColumnName))
        {
            constraints.Add($"REFERENCES \"{column.ForeignKeyMetadata.ReferencedTableName}\"(\"{column.ForeignKeyMetadata.ReferencedColumnName}\")");
        }

        if (constraints.Count > 0)
        {
            sb.Append(" ");
            sb.Append(string.Join(" ", constraints));
        }

        return sb.ToString();
    }

    private static string GenerateDataType(ColumnMetadata column)
    {
        var dataType = column.DataType ?? SqlDataType.Varchar;

        return dataType switch
        {
            SqlDataType.Int => column.IsAutoIncrement && column.IsPrimaryKey
                ? "BIGSERIAL"  // Use BIGSERIAL for auto-increment primary keys
                : "BIGINT",     // Use BIGINT for regular integers (PostgreSQL convention)

            SqlDataType.Decimal => column.Precision.HasValue && column.Scale.HasValue
                ? $"DECIMAL({column.Precision.Value}, {column.Scale.Value})"
                : "DECIMAL",

            SqlDataType.Float => "DOUBLE PRECISION",

            SqlDataType.Varchar => column.Length.HasValue
                ? $"VARCHAR({column.Length.Value})"
                : "VARCHAR",

            SqlDataType.Char => column.Length.HasValue
                ? $"CHAR({column.Length.Value})"
                : "CHAR(1)",

            SqlDataType.Text => "TEXT",

            SqlDataType.TimestampWithTimeZone => "TIMESTAMP WITH TIME ZONE",

            SqlDataType.TimestampWithoutTimeZone => "TIMESTAMP WITHOUT TIME ZONE",

            _ => "VARCHAR"
        };
    }

    private static string EscapeDefaultValue(string value, SqlDataType? dataType)
    {
        // For string types, wrap in single quotes and escape single quotes
        if (dataType == SqlDataType.Varchar || 
            dataType == SqlDataType.Char || 
            dataType == SqlDataType.Text ||
            !dataType.HasValue)
        {
            return $"'{value.Replace("'", "''")}'";
        }

        // For numeric types, return as-is
        if (dataType == SqlDataType.Int || 
            dataType == SqlDataType.Decimal || 
            dataType == SqlDataType.Float)
        {
            return value;
        }

        // For timestamp types, wrap in single quotes
        if (dataType == SqlDataType.TimestampWithTimeZone || 
            dataType == SqlDataType.TimestampWithoutTimeZone)
        {
            return $"'{value.Replace("'", "''")}'";
        }

        return $"'{value.Replace("'", "''")}'";
    }
}

