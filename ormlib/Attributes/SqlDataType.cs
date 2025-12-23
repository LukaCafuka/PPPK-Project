namespace OrmLib.Attributes;

/// <summary>
/// Represents SQL data types supported by the ORM.
/// </summary>
public enum SqlDataType
{
    /// <summary>
    /// Integer type (INT)
    /// </summary>
    Int,

    /// <summary>
    /// Decimal type with precision and scale (DECIMAL)
    /// </summary>
    Decimal,

    /// <summary>
    /// Floating point type (FLOAT)
    /// </summary>
    Float,

    /// <summary>
    /// Variable-length character string (VARCHAR)
    /// </summary>
    Varchar,

    /// <summary>
    /// Fixed-length character string (CHAR)
    /// </summary>
    Char,

    /// <summary>
    /// Text type (TEXT)
    /// </summary>
    Text,

    /// <summary>
    /// Timestamp with timezone (TIMESTAMP WITH TIME ZONE)
    /// </summary>
    TimestampWithTimeZone,

    /// <summary>
    /// Timestamp without timezone (TIMESTAMP WITHOUT TIME ZONE)
    /// </summary>
    TimestampWithoutTimeZone
}

