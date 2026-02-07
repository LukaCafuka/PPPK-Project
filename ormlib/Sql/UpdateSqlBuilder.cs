using System.Text;
using System.Linq;
using OrmLib.Metadata;

namespace OrmLib.Sql;

/// <summary>
/// Builds PostgreSQL UPDATE SQL statements.
/// </summary>
public static class UpdateSqlBuilder
{
    /// <summary>
    /// Generates an UPDATE statement for the specified table metadata and changed columns.
    /// </summary>
    /// <param name="tableMetadata">The table metadata.</param>
    /// <param name="changedColumns">The columns that have been modified.</param>
    /// <returns>A tuple containing the SQL statement and the list of changed columns.</returns>
    public static (string Sql, IReadOnlyList<ColumnMetadata> Columns) BuildUpdate(
        TableMetadata tableMetadata,
        IEnumerable<ColumnMetadata> changedColumns)
    {
        if (tableMetadata == null)
            throw new ArgumentNullException(nameof(tableMetadata));

        var changedColumnsList = changedColumns?.ToList() ?? throw new ArgumentNullException(nameof(changedColumns));

        if (changedColumnsList.Count == 0)
        {
            throw new InvalidOperationException("No columns specified for update.");
        }

        // Don't update primary key columns
        var columnsToUpdate = changedColumnsList
            .Where(c => !c.IsPrimaryKey)
            .ToList();

        if (columnsToUpdate.Count == 0)
        {
            throw new InvalidOperationException("Cannot update primary key column.");
        }

        var sql = new StringBuilder();
        sql.Append($"UPDATE \"{tableMetadata.TableName}\" SET ");

        var setClauses = new List<string>();
        for (int i = 0; i < columnsToUpdate.Count; i++)
        {
            setClauses.Add($"\"{columnsToUpdate[i].ColumnName}\" = @p{i + 1}");
        }

        sql.Append(string.Join(", ", setClauses));

        // Add WHERE clause for primary key
        sql.Append($" WHERE \"{tableMetadata.PrimaryKey.ColumnName}\" = @p{columnsToUpdate.Count + 1}");

        return (sql.ToString(), columnsToUpdate);
    }
}

