using System.Data;
using Npgsql;
using OrmLib.Migrations;

namespace OrmLib.Migrations;

/// <summary>
/// Reads the current database schema from PostgreSQL.
/// </summary>
internal static class SchemaReader
{
    /// <summary>
    /// Reads the current database schema.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    /// <returns>A schema snapshot.</returns>
    public static SchemaSnapshot ReadSchema(NpgsqlConnection connection)
    {
        var tables = new List<TableSnapshot>();

        // Query to get all tables in the current schema (excluding system tables)
        var tablesQuery = @"
            SELECT table_name 
            FROM information_schema.tables 
            WHERE table_schema = 'public' 
            AND table_type = 'BASE TABLE'
            AND table_name NOT LIKE '__migrations%'
            ORDER BY table_name";

        using var tablesCommand = new NpgsqlCommand(tablesQuery, connection);
        using var tablesReader = tablesCommand.ExecuteReader();

        var tableNames = new List<string>();
        while (tablesReader.Read())
        {
            tableNames.Add(tablesReader.GetString(0));
        }
        tablesReader.Close();

        // For each table, read its columns
        foreach (var tableName in tableNames)
        {
            var tableSnapshot = ReadTableSchema(connection, tableName);
            if (tableSnapshot != null)
            {
                tables.Add(tableSnapshot);
            }
        }

        return new SchemaSnapshot(tables);
    }

    private static TableSnapshot? ReadTableSchema(NpgsqlConnection connection, string tableName)
    {
        // First, read foreign key constraints for this table
        var foreignKeys = ReadForeignKeys(connection, tableName);

        // Query to get column information
        var columnsQuery = @"
            SELECT 
                c.column_name,
                c.data_type,
                c.character_maximum_length,
                c.numeric_precision,
                c.numeric_scale,
                c.is_nullable,
                c.column_default,
                CASE WHEN pk.column_name IS NOT NULL THEN true ELSE false END as is_primary_key,
                CASE WHEN uq.column_name IS NOT NULL THEN true ELSE false END as is_unique
            FROM information_schema.columns c
            LEFT JOIN (
                SELECT ku.table_name, ku.column_name
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage ku 
                    ON tc.constraint_name = ku.constraint_name
                WHERE tc.constraint_type = 'PRIMARY KEY'
            ) pk ON c.table_name = pk.table_name AND c.column_name = pk.column_name
            LEFT JOIN (
                SELECT ku.table_name, ku.column_name
                FROM information_schema.table_constraints tc
                JOIN information_schema.key_column_usage ku 
                    ON tc.constraint_name = ku.constraint_name
                WHERE tc.constraint_type = 'UNIQUE'
            ) uq ON c.table_name = uq.table_name AND c.column_name = uq.column_name
            WHERE c.table_schema = 'public' 
            AND c.table_name = @tableName
            ORDER BY c.ordinal_position";

        using var command = new NpgsqlCommand(columnsQuery, connection);
        command.Parameters.AddWithValue("@tableName", tableName);

        using var reader = command.ExecuteReader();

        var columns = new List<ColumnSnapshot>();
        string? primaryKeyColumn = null;

        while (reader.Read())
        {
            var columnName = reader.GetString(0);
            var dataType = reader.GetString(1);
            var maxLength = reader.IsDBNull(2) ? (int?)null : reader.GetInt32(2);
            var precision = reader.IsDBNull(3) ? (int?)null : reader.GetInt32(3);
            var scale = reader.IsDBNull(4) ? (int?)null : reader.GetInt32(4);
            var isNullable = reader.GetString(5) == "YES";
            var defaultValue = reader.IsDBNull(6) ? null : reader.GetString(6);
            var isPrimaryKey = reader.GetBoolean(7);
            var isUnique = reader.GetBoolean(8);

            // Format data type with length/precision/scale
            var fullDataType = FormatDataType(dataType, maxLength, precision, scale);

            // Look up FK info for this column
            string? referencedTable = null;
            string? referencedColumn = null;
            if (foreignKeys.TryGetValue(columnName, out var fkInfo))
            {
                referencedTable = fkInfo.ReferencedTable;
                referencedColumn = fkInfo.ReferencedColumn;
            }

            columns.Add(new ColumnSnapshot(
                columnName,
                fullDataType,
                isNullable,
                isUnique,
                defaultValue,
                referencedTable,
                referencedColumn));

            if (isPrimaryKey)
            {
                primaryKeyColumn = columnName;
            }
        }

        if (columns.Count == 0)
            return null;

        return new TableSnapshot(tableName, columns, primaryKeyColumn);
    }

    /// <summary>
    /// Reads foreign key constraints for a given table.
    /// Returns a dictionary mapping column name to (referenced table, referenced column).
    /// </summary>
    private static Dictionary<string, (string ReferencedTable, string ReferencedColumn)> ReadForeignKeys(
        NpgsqlConnection connection, string tableName)
    {
        var result = new Dictionary<string, (string, string)>(StringComparer.OrdinalIgnoreCase);

        var fkQuery = @"
            SELECT
                kcu.column_name,
                ccu.table_name AS referenced_table,
                ccu.column_name AS referenced_column
            FROM information_schema.table_constraints tc
            JOIN information_schema.key_column_usage kcu
                ON tc.constraint_name = kcu.constraint_name
                AND tc.table_schema = kcu.table_schema
            JOIN information_schema.constraint_column_usage ccu
                ON tc.constraint_name = ccu.constraint_name
                AND tc.table_schema = ccu.table_schema
            WHERE tc.constraint_type = 'FOREIGN KEY'
            AND tc.table_schema = 'public'
            AND tc.table_name = @tableName";

        using var command = new NpgsqlCommand(fkQuery, connection);
        command.Parameters.AddWithValue("@tableName", tableName);

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var columnName = reader.GetString(0);
            var referencedTable = reader.GetString(1);
            var referencedColumn = reader.GetString(2);
            result[columnName] = (referencedTable, referencedColumn);
        }

        return result;
    }

    private static string FormatDataType(string dataType, int? maxLength, int? precision, int? scale)
    {
        return dataType.ToUpper() switch
        {
            "CHARACTER VARYING" or "VARCHAR" => maxLength.HasValue ? $"VARCHAR({maxLength.Value})" : "VARCHAR",
            "CHARACTER" or "CHAR" => maxLength.HasValue ? $"CHAR({maxLength.Value})" : "CHAR",
            "NUMERIC" or "DECIMAL" => precision.HasValue && scale.HasValue 
                ? $"DECIMAL({precision.Value},{scale.Value})" 
                : precision.HasValue 
                    ? $"DECIMAL({precision.Value})" 
                    : "DECIMAL",
            "DOUBLE PRECISION" => "FLOAT",
            "BIGINT" => "BIGINT", // Keep as BIGINT (BIGSERIAL is stored as BIGINT in information_schema)
            "TIMESTAMP WITHOUT TIME ZONE" => "TIMESTAMP WITHOUT TIME ZONE",
            "TIMESTAMP WITH TIME ZONE" => "TIMESTAMP WITH TIME ZONE",
            _ => dataType.ToUpper()
        };
    }
}

