namespace OrmLib.Attributes;

/// <summary>
/// Specifies the database table name for a class.
/// When applied to a class, indicates that the class represents a database table.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
public sealed class TableAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the database table.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TableAttribute"/> class with the specified table name.
    /// </summary>
    /// <param name="name">The name of the database table.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is null or empty.</exception>
    public TableAttribute(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Table name cannot be null or empty.", nameof(name));

        Name = name;
    }
}

