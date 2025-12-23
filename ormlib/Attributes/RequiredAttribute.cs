namespace OrmLib.Attributes;

/// <summary>
/// Specifies that a property is required and maps to a NOT NULL column in the database.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class RequiredAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RequiredAttribute"/> class.
    /// </summary>
    public RequiredAttribute()
    {
    }
}

