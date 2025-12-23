namespace OrmLib.Attributes;

/// <summary>
/// Specifies that a primary key property should auto-increment in the database.
/// This attribute should be used together with <see cref="KeyAttribute"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class AutoIncrementAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AutoIncrementAttribute"/> class.
    /// </summary>
    public AutoIncrementAttribute()
    {
    }
}

