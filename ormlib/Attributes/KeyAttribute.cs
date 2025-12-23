namespace OrmLib.Attributes;

/// <summary>
/// Specifies that a property is a primary key in the database table.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class KeyAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="KeyAttribute"/> class.
    /// </summary>
    public KeyAttribute()
    {
    }
}

