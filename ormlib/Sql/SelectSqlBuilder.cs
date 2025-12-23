using System.Text;
using System.Linq;
using OrmLib.Metadata;

namespace OrmLib.Sql;

/// <summary>
/// Builds PostgreSQL SELECT SQL statements.
/// </summary>
public static class SelectSqlBuilder
{
    /// <summary>
    /// Generates a SELECT statement for the specified table metadata.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <param name="whereClause">Optional WHERE clause (without the WHERE keyword).</param>
    /// <param name="orderByClause">Optional ORDER BY clause (without the ORDER BY keyword).</param>
    /// <returns>The SELECT SQL statement.</returns>
    public static string BuildSelect(
        TableMetadata tableMetadata,
        string? whereClause = null,
        string? orderByClause = null)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        var sql = new StringBuilder();
        sql.Append("SELECT ");

        // Select all columns
        var columnNames = tableMetadata.Columns
            .Select(c => $"\"{tableMetadata.TableName}\".\"{c.ColumnName}\"")
            .ToList();

        sql.Append(string.Join(", ", columnNames));
        sql.Append($" FROM \"{tableMetadata.TableName}\"");

        if (!string.IsNullOrWhiteSpace(whereClause))
        {
            sql.Append($" WHERE {whereClause}");
        }

        if (!string.IsNullOrWhiteSpace(orderByClause))
        {
            sql.Append($" ORDER BY {orderByClause}");
        }

        return sql.ToString();
    }

    /// <summary>
    /// Generates a SELECT statement for finding an entity by primary key.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <returns>The SELECT SQL statement.</returns>
    public static string BuildSelectByPrimaryKey(TableMetadata tableMetadata)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        var whereClause = $"\"{tableMetadata.PrimaryKey.ColumnName}\" = $1";
        return BuildSelect(tableMetadata, whereClause);
    }

    /// <summary>
    /// Generates a SELECT statement for all rows in a table.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <param name="orderByClause">Optional ORDER BY clause (without the ORDER BY keyword).</param>
    /// <returns>The SELECT SQL statement.</returns>
    public static string BuildSelectAll(TableMetadata tableMetadata, string? orderByClause = null)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        return BuildSelect(tableMetadata, null, orderByClause);
    }
}

