namespace OrmLib.Attributes;

/// <summary>
/// Specifies the database column name and type for a property.
/// When applied to a property, indicates that the property maps to a database column.
/// </summary>
[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
public sealed class ColumnAttribute : Attribute
{
    /// <summary>
    /// Gets the name of the database column.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Gets the SQL data type for the column.
    /// </summary>
    public SqlDataType? DataType { get; }

    /// <summary>
    /// Gets the length/precision for VARCHAR or CHAR types.
    /// </summary>
    public int? Length { get; set; }

    /// <summary>
    /// Gets the precision for DECIMAL type.
    /// </summary>
    public int? Precision { get; set; }

    /// <summary>
    /// Gets the scale for DECIMAL type.
    /// </summary>
    public int? Scale { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class.
    /// </summary>
    public ColumnAttribute()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class with the specified column name.
    /// </summary>
    /// <param name="name">The name of the database column.</param>
    public ColumnAttribute(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class with the specified column name and data type.
    /// </summary>
    /// <param name="name">The name of the database column.</param>
    /// <param name="dataType">The SQL data type for the column.</param>
    public ColumnAttribute(string name, SqlDataType dataType)
    {
        Name = name;
        DataType = dataType;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnAttribute"/> class with the specified column name, data type, and length.
    /// </summary>
    /// <param name="name">The name of the database column.</param>
    /// <param name="dataType">The SQL data type for the column.</param>
    /// <param name="length">The length/precision for VARCHAR or CHAR types.</param>
    public ColumnAttribute(string name, SqlDataType dataType, int length)
    {
        Name = name;
        DataType = dataType;
        Length = length;
    }
}

