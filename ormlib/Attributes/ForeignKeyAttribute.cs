namespace OrmLib.Attributes;

/// <summary>
/// Specifies that a property represents a foreign key relationship.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ForeignKeyAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the navigation property that this foreign key references.
    /// </summary>
    public string? NavigationProperty { get; }

    /// <summary>
    /// Gets the name of the referenced table.
    /// </summary>
    public string? ReferencedTable { get; }

    /// <summary>
    /// Gets the name of the referenced column (usually the primary key).
    /// </summary>
    public string? ReferencedColumn { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyAttribute"/> class.
    /// </summary>
    public ForeignKeyAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyAttribute"/> class with the specified navigation property name.
    /// </summary>
    /// <param name="navigationProperty">The name of the navigation property that this foreign key references.</param>
    public ForeignKeyAttribute(string navigationProperty)
    {
        NavigationProperty = navigationProperty;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ForeignKeyAttribute"/> class with the specified referenced table and column.
    /// </summary>
    /// <param name="referencedTable">The name of the referenced table.</param>
    /// <param name="referencedColumn">The name of the referenced column (usually the primary key).</param>
    public ForeignKeyAttribute(string referencedTable, string referencedColumn)
    {
        ReferencedTable = referencedTable;
        ReferencedColumn = referencedColumn;
    }
}

