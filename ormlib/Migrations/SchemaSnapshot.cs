using System.Collections.ObjectModel;
using OrmLib.Metadata;

namespace OrmLib.Migrations;

/// <summary>
/// Represents a snapshot of the database schema at a point in time.
/// </summary>
public sealed class SchemaSnapshot
{
    /// <summary>
    /// Gets the collection of table snapshots.
    /// </summary>
    public ReadOnlyCollection<TableSnapshot> Tables { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="SchemaSnapshot"/> class.
    /// </summary>
    /// <param name="tables">The table snapshots.</param>
    public SchemaSnapshot(IEnumerable<TableSnapshot> tables)
    {
        Tables = (tables?.ToList() ?? throw new ArgumentNullException(nameof(tables))).AsReadOnly();
    }

    /// <summary>
    /// Gets a table snapshot by table name.
    /// </summary>
    /// <param name="tableName">The table name.</param>
    /// <returns>The table snapshot, or null if not found.</returns>
    public TableSnapshot? GetTable(string tableName)
    {
        return Tables.FirstOrDefault(t => t.TableName.Equals(tableName, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Represents a snapshot of a database table.
/// </summary>
public sealed class TableSnapshot
{
    /// <summary>
    /// Gets the table name.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Gets the collection of column snapshots.
    /// </summary>
    public ReadOnlyCollection<ColumnSnapshot> Columns { get; }

    /// <summary>
    /// Gets the primary key column name.
    /// </summary>
    public string? PrimaryKeyColumn { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableSnapshot"/> class.
    /// </summary>
    /// <param name="tableName">The table name.</param>
    /// <param name="columns">The column snapshots.</param>
    /// <param name="primaryKeyColumn">The primary key column name.</param>
    public TableSnapshot(string tableName, IEnumerable<ColumnSnapshot> columns, string? primaryKeyColumn = null)
    {
        TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        Columns = (columns?.ToList() ?? throw new ArgumentNullException(nameof(columns))).AsReadOnly();
        PrimaryKeyColumn = primaryKeyColumn;
    }
}

/// <summary>
/// Represents a snapshot of a database column.
/// </summary>
public sealed class ColumnSnapshot
{
    /// <summary>
    /// Gets the column name.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the SQL data type.
    /// </summary>
    public string DataType { get; }

    /// <summary>
    /// Gets a value indicating whether the column is nullable.
    /// </summary>
    public bool IsNullable { get; }

    /// <summary>
    /// Gets a value indicating whether the column is unique.
    /// </summary>
    public bool IsUnique { get; }

    /// <summary>
    /// Gets the default value, or null if not specified.
    /// </summary>
    public string? DefaultValue { get; }

    /// <summary>
    /// Gets the referenced table name for a foreign key column, or null if not a foreign key.
    /// </summary>
    public string? ReferencedTable { get; }

    /// <summary>
    /// Gets the referenced column name for a foreign key column, or null if not a foreign key.
    /// </summary>
    public string? ReferencedColumn { get; }

    /// <summary>
    /// Gets a value indicating whether this column is a foreign key.
    /// </summary>
    public bool IsForeignKey => ReferencedTable != null && ReferencedColumn != null;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnSnapshot"/> class.
    /// </summary>
    public ColumnSnapshot(
        string columnName,
        string dataType,
        bool isNullable,
        bool isUnique = false,
        string? defaultValue = null,
        string? referencedTable = null,
        string? referencedColumn = null)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        DataType = dataType ?? throw new ArgumentNullException(nameof(dataType));
        IsNullable = isNullable;
        IsUnique = isUnique;
        DefaultValue = defaultValue;
        ReferencedTable = referencedTable;
        ReferencedColumn = referencedColumn;
    }
}

