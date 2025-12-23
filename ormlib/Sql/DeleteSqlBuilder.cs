using System.Text;
using OrmLib.Metadata;

namespace OrmLib.Sql;

/// <summary>
/// Builds PostgreSQL DELETE SQL statements.
/// </summary>
public static class DeleteSqlBuilder
{
    /// <summary>
    /// Generates a DELETE statement for the specified table metadata.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <returns>The DELETE SQL statement.</returns>
    public static string BuildDelete(TableMetadata tableMetadata)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        var sql = new StringBuilder();
        sql.Append($"DELETE FROM \"{tableMetadata.TableName}\"");
        sql.Append($" WHERE \"{tableMetadata.PrimaryKey.ColumnName}\" = $1");

        return sql.ToString();
    }

    /// <summary>
    /// Generates a DELETE statement with a custom WHERE clause.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <param name="whereClause">The WHERE clause (without the WHERE keyword).</param>
    /// <returns>The DELETE SQL statement.</returns>
    public static string BuildDeleteWithWhere(TableMetadata tableMetadata, string whereClause)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        if (string.IsNullOrWhiteSpace(whereClause))
            throw new ArgumentException("WHERE clause cannot be null or empty.", nameof(whereClause));

        var sql = new StringBuilder();
        sql.Append($"DELETE FROM \"{tableMetadata.TableName}\"");
        sql.Append($" WHERE {whereClause}");

        return sql.ToString();
    }
}

