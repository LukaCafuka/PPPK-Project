namespace OrmLib.Attributes;

/// <summary>
/// Specifies a default value for a database column.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class DefaultAttribute : Attribute
{
    /// <summary>
    /// Gets the default value as a string representation.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Gets a value indicating whether the default value is a SQL expression (true) or a literal value (false).
    /// </summary>
    public bool IsSqlExpression { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAttribute"/> class with a literal default value.
    /// </summary>
    /// <param name="value">The default value as a string.</param>
    public DefaultAttribute(string value)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsSqlExpression = false;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DefaultAttribute"/> class with a SQL expression or literal value.
    /// </summary>
    /// <param name="value">The default value as a string.</param>
    /// <param name="isSqlExpression">True if the value is a SQL expression (e.g., "CURRENT_TIMESTAMP"), false if it's a literal.</param>
    public DefaultAttribute(string value, bool isSqlExpression)
    {
        Value = value ?? throw new ArgumentNullException(nameof(value));
        IsSqlExpression = isSqlExpression;
    }
}

