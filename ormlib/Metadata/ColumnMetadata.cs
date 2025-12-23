using System.Reflection;
using OrmLib.Attributes;

namespace OrmLib.Metadata;

/// <summary>
/// Represents metadata for a database column extracted from a property.
/// </summary>
public sealed class ColumnMetadata
{
    /// <summary>
    /// Gets the property information for this column.
    /// </summary>
    public PropertyInfo Property { get; }

    /// <summary>
    /// Gets the name of the database column.
    /// </summary>
    public string ColumnName { get; }

    /// <summary>
    /// Gets the SQL data type for the column, or null if not specified.
    /// </summary>
    public SqlDataType? DataType { get; }

    /// <summary>
    /// Gets the length/precision for VARCHAR or CHAR types, or null if not specified.
    /// </summary>
    public int? Length { get; }

    /// <summary>
    /// Gets the precision for DECIMAL type, or null if not specified.
    /// </summary>
    public int? Precision { get; }

    /// <summary>
    /// Gets the scale for DECIMAL type, or null if not specified.
    /// </summary>
    public int? Scale { get; }

    /// <summary>
    /// Gets a value indicating whether this column is a primary key.
    /// </summary>
    public bool IsPrimaryKey { get; }

    /// <summary>
    /// Gets a value indicating whether this column is required (NOT NULL).
    /// </summary>
    public bool IsRequired { get; }

    /// <summary>
    /// Gets a value indicating whether this column is unique.
    /// </summary>
    public bool IsUnique { get; }

    /// <summary>
    /// Gets a value indicating whether this column auto-increments.
    /// </summary>
    public bool IsAutoIncrement { get; }

    /// <summary>
    /// Gets the default value for this column, or null if not specified.
    /// </summary>
    public string? DefaultValue { get; }

    /// <summary>
    /// Gets a value indicating whether the default value is a SQL expression.
    /// </summary>
    public bool IsDefaultSqlExpression { get; }

    /// <summary>
    /// Gets a value indicating whether this column is a foreign key.
    /// </summary>
    public bool IsForeignKey => ForeignKeyMetadata != null;

    /// <summary>
    /// Gets the foreign key metadata for this column, or null if it's not a foreign key.
    /// </summary>
    public RelationshipMetadata? ForeignKeyMetadata { get; internal set; }

    /// <summary>
    /// Gets the CLR type of the property.
    /// </summary>
    public Type PropertyType => Property.PropertyType;

    /// <summary>
    /// Gets the property name.
    /// </summary>
    public string PropertyName => Property.Name;

    /// <summary>
    /// Initializes a new instance of the <see cref="ColumnMetadata"/> class.
    /// </summary>
    /// <param name="property">The property information.</param>
    /// <param name="columnName">The database column name.</param>
    /// <param name="dataType">The SQL data type, or null if not specified.</param>
    /// <param name="length">The length/precision, or null if not specified.</param>
    /// <param name="precision">The precision for DECIMAL, or null if not specified.</param>
    /// <param name="scale">The scale for DECIMAL, or null if not specified.</param>
    /// <param name="isPrimaryKey">True if this is a primary key.</param>
    /// <param name="isRequired">True if this column is required (NOT NULL).</param>
    /// <param name="isUnique">True if this column is unique.</param>
    /// <param name="isAutoIncrement">True if this column auto-increments.</param>
    /// <param name="defaultValue">The default value, or null if not specified.</param>
    /// <param name="isDefaultSqlExpression">True if the default value is a SQL expression.</param>
    internal ColumnMetadata(
        PropertyInfo property,
        string columnName,
        SqlDataType? dataType = null,
        int? length = null,
        int? precision = null,
        int? scale = null,
        bool isPrimaryKey = false,
        bool isRequired = false,
        bool isUnique = false,
        bool isAutoIncrement = false,
        string? defaultValue = null,
        bool isDefaultSqlExpression = false)
    {
        Property = property ?? throw new ArgumentNullException(nameof(property));
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        DataType = dataType;
        Length = length;
        Precision = precision;
        Scale = scale;
        IsPrimaryKey = isPrimaryKey;
        IsRequired = isRequired;
        IsUnique = isUnique;
        IsAutoIncrement = isAutoIncrement;
        DefaultValue = defaultValue;
        IsDefaultSqlExpression = isDefaultSqlExpression;
    }

    /// <summary>
    /// Gets the value of this column from the specified entity instance.
    /// </summary>
    /// <param name="entity">The entity instance.</param>
    /// <returns>The column value.</returns>
    public object? GetValue(object entity)
    {
        return Property.GetValue(entity);
    }

    /// <summary>
    /// Sets the value of this column on the specified entity instance.
    /// </summary>
    /// <param name="entity">The entity instance.</param>
    /// <param name="value">The value to set.</param>
    public void SetValue(object entity, object? value)
    {
        Property.SetValue(entity, value);
    }
}

