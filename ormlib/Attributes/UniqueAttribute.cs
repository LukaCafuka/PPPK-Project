namespace OrmLib.Attributes;

/// <summary>
/// Specifies that a property maps to a UNIQUE column in the database.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class UniqueAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UniqueAttribute"/> class.
    /// </summary>
    public UniqueAttribute()
    {
    }
}

