using System.Collections.ObjectModel;
using System.Reflection;
using System.Linq;
using OrmLib.Attributes;

namespace OrmLib.Metadata;

/// <summary>
/// Represents metadata for a database table extracted from a class.
/// </summary>
public sealed class TableMetadata
{
    private readonly List<ColumnMetadata> _columns;
    private readonly List<RelationshipMetadata> _relationships;

    /// <summary>
    /// Gets the type of the entity class.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    /// Gets the name of the database table.
    /// </summary>
    public string TableName { get; }

    /// <summary>
    /// Gets a read-only collection of column metadata.
    /// </summary>
    public ReadOnlyCollection<ColumnMetadata> Columns { get; }

    /// <summary>
    /// Gets a read-only collection of relationship metadata.
    /// </summary>
    public ReadOnlyCollection<RelationshipMetadata> Relationships { get; }

    /// <summary>
    /// Gets the primary key column metadata.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when no primary key is found.</exception>
    public ColumnMetadata PrimaryKey
    {
        get
        {
            var pk = _columns.FirstOrDefault(c => c.IsPrimaryKey);
            if (pk == null)
                throw new InvalidOperationException($"Entity type {EntityType.Name} does not have a primary key defined.");

            return pk;
        }
    }

    /// <summary>
    /// Gets all foreign key columns.
    /// </summary>
    public IEnumerable<ColumnMetadata> ForeignKeyColumns => _columns.Where(c => c.IsForeignKey);

    /// <summary>
    /// Initializes a new instance of the <see cref="TableMetadata"/> class.
    /// </summary>
    /// <param name="entityType">The entity type.</param>
    /// <param name="tableName">The database table name.</param>
    /// <param name="columns">The column metadata.</param>
    /// <param name="relationships">The relationship metadata.</param>
    internal TableMetadata(
        Type entityType,
        string tableName,
        IEnumerable<ColumnMetadata> columns,
        IEnumerable<RelationshipMetadata> relationships)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));

        _columns = columns?.ToList() ?? throw new ArgumentNullException(nameof(columns));
        _relationships = relationships?.ToList() ?? throw new ArgumentNullException(nameof(relationships));

        Columns = new ReadOnlyCollection<ColumnMetadata>(_columns);
        Relationships = new ReadOnlyCollection<RelationshipMetadata>(_relationships);

        // Validate that there's exactly one primary key
        var primaryKeyCount = _columns.Count(c => c.IsPrimaryKey);
        if (primaryKeyCount == 0)
            throw new InvalidOperationException($"Entity type {EntityType.Name} must have exactly one primary key defined.");

        if (primaryKeyCount > 1)
            throw new InvalidOperationException($"Entity type {EntityType.Name} has multiple primary keys defined. Only one primary key is supported.");
    }

    /// <summary>
    /// Gets a column metadata by property name.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <returns>The column metadata, or null if not found.</returns>
    public ColumnMetadata? GetColumnByPropertyName(string propertyName)
    {
        return _columns.FirstOrDefault(c => c.PropertyName == propertyName);
    }

    /// <summary>
    /// Gets a column metadata by column name.
    /// </summary>
    /// <param name="columnName">The column name.</param>
    /// <returns>The column metadata, or null if not found.</returns>
    public ColumnMetadata? GetColumnByColumnName(string columnName)
    {
        return _columns.FirstOrDefault(c => c.ColumnName.Equals(columnName, StringComparison.OrdinalIgnoreCase));
    }
}

