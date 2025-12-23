using System.Reflection;
using OrmLib.Attributes;

namespace OrmLib.Metadata;

/// <summary>
/// Represents metadata for a foreign key relationship between tables.
/// </summary>
public sealed class RelationshipMetadata
{
    /// <summary>
    /// Gets the property that represents the foreign key column.
    /// </summary>
    public PropertyInfo ForeignKeyProperty { get; }

    /// <summary>
    /// Gets the name of the navigation property that this foreign key references, or null if not specified.
    /// </summary>
    public string? NavigationPropertyName { get; internal set; }

    /// <summary>
    /// Gets the name of the referenced table, or null if not specified.
    /// </summary>
    public string? ReferencedTableName { get; internal set; }

    /// <summary>
    /// Gets the name of the referenced column (usually the primary key), or null if not specified.
    /// </summary>
    public string? ReferencedColumnName { get; internal set; }

    /// <summary>
    /// Gets the type of the referenced entity.
    /// </summary>
    public Type? ReferencedEntityType { get; internal set; }

    /// <summary>
    /// Gets the navigation property that this foreign key references, or null if not found.
    /// </summary>
    public PropertyInfo? NavigationProperty { get; internal set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="RelationshipMetadata"/> class.
    /// </summary>
    /// <param name="foreignKeyProperty">The property that represents the foreign key column.</param>
    /// <param name="navigationPropertyName">The name of the navigation property, or null.</param>
    /// <param name="referencedTableName">The name of the referenced table, or null.</param>
    /// <param name="referencedColumnName">The name of the referenced column, or null.</param>
    internal RelationshipMetadata(
        PropertyInfo foreignKeyProperty,
        string? navigationPropertyName = null,
        string? referencedTableName = null,
        string? referencedColumnName = null)
    {
        ForeignKeyProperty = foreignKeyProperty ?? throw new ArgumentNullException(nameof(foreignKeyProperty));
        NavigationPropertyName = navigationPropertyName;
        ReferencedTableName = referencedTableName;
        ReferencedColumnName = referencedColumnName;
    }
}

