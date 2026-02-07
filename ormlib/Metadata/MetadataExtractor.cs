using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Linq;
using OrmLib.Attributes;

namespace OrmLib.Metadata;

/// <summary>
/// Extracts metadata from entity types using reflection.
/// </summary>
public static class MetadataExtractor
{
    private static readonly ConcurrentDictionary<Type, TableMetadata> _metadataCache = new();

    /// <summary>
    /// Extracts table metadata from the specified entity type.
    /// Results are cached for performance.
    /// </summary>
    /// <param name="entityType">The entity type to extract metadata from.</param>
    /// <returns>The table metadata.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="entityType"/> is null.</exception>
    public static TableMetadata ExtractTableMetadata([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type entityType)
    {
        if (entityType == null)
            throw new ArgumentNullException(nameof(entityType));

        return _metadataCache.GetOrAdd(entityType, ExtractTableMetadataInternal);
    }

    /// <summary>
    /// Extracts table metadata from the specified entity type.
    /// Results are cached for performance.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <returns>The table metadata.</returns>
    public static TableMetadata ExtractTableMetadata<T>()
    {
        return ExtractTableMetadata(typeof(T));
    }

    private static TableMetadata ExtractTableMetadataInternal([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)] Type entityType)
    {
        // Extract table name
        var tableAttribute = entityType.GetCustomAttribute<TableAttribute>();
        var tableName = tableAttribute?.Name ?? entityType.Name;

        // Extract all properties
        var properties = entityType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0)
            .ToList();

        var columns = new List<ColumnMetadata>();
        var relationships = new List<RelationshipMetadata>();

        foreach (var property in properties)
        {
            // Skip navigation properties (they are typically reference types that are not primitives or strings)
            // We'll identify them later when processing foreign keys
            if (IsNavigationProperty(property))
                continue;

            var columnMetadata = ExtractColumnMetadata(property);
            columns.Add(columnMetadata);

            // Extract foreign key metadata if present
            var foreignKeyAttribute = property.GetCustomAttribute<ForeignKeyAttribute>();
            if (foreignKeyAttribute != null)
            {
                var relationshipMetadata = ExtractRelationshipMetadata(
                    property,
                    foreignKeyAttribute,
                    entityType);

                relationships.Add(relationshipMetadata);
                columnMetadata.ForeignKeyMetadata = relationshipMetadata;
            }
        }

        // Resolve navigation properties and referenced types for relationships
        ResolveRelationships(entityType, relationships, columns);

        return new TableMetadata(entityType, tableName, columns, relationships);
    }

    private static ColumnMetadata ExtractColumnMetadata(PropertyInfo property)
    {
        // Extract column name
        var columnAttribute = property.GetCustomAttribute<ColumnAttribute>();
        var columnName = columnAttribute?.Name ?? property.Name;

        // Extract data type information
        var dataType = columnAttribute?.DataType;
        var length = columnAttribute?.Length;
        var precision = columnAttribute?.Precision;
        var scale = columnAttribute?.Scale;

        // If data type is not specified, infer it from the property type
        if (!dataType.HasValue)
        {
            dataType = InferSqlDataType(property.PropertyType);
        }

        // Extract constraints
        var isPrimaryKey = property.GetCustomAttribute<KeyAttribute>() != null;
        var isRequired = property.GetCustomAttribute<RequiredAttribute>() != null ||
                         IsNonNullableValueType(property.PropertyType);
        var isUnique = property.GetCustomAttribute<UniqueAttribute>() != null;
        var isAutoIncrement = property.GetCustomAttribute<AutoIncrementAttribute>() != null;

        // Extract default value
        var defaultAttribute = property.GetCustomAttribute<DefaultAttribute>();
        var defaultValue = defaultAttribute?.Value;
        var isDefaultSqlExpression = defaultAttribute?.IsSqlExpression ?? false;

        return new ColumnMetadata(
            property,
            columnName,
            dataType,
            length,
            precision,
            scale,
            isPrimaryKey,
            isRequired,
            isUnique,
            isAutoIncrement,
            defaultValue,
            isDefaultSqlExpression);
    }

    private static RelationshipMetadata ExtractRelationshipMetadata(
        PropertyInfo foreignKeyProperty,
        ForeignKeyAttribute foreignKeyAttribute,
        Type entityType)
    {
        return new RelationshipMetadata(
            foreignKeyProperty,
            foreignKeyAttribute.NavigationProperty,
            foreignKeyAttribute.ReferencedTable,
            foreignKeyAttribute.ReferencedColumn);
    }

    private static void ResolveRelationships(
        Type entityType,
        List<RelationshipMetadata> relationships,
        List<ColumnMetadata> columns)
    {
        foreach (var relationship in relationships)
        {
            // Try to resolve navigation property
            if (!string.IsNullOrEmpty(relationship.NavigationPropertyName))
            {
                var navProperty = entityType.GetProperty(
                    relationship.NavigationPropertyName,
                    BindingFlags.Public | BindingFlags.Instance);

                if (navProperty != null)
                {
                    relationship.NavigationProperty = navProperty;
                    relationship.ReferencedEntityType = GetEntityTypeFromNavigationProperty(navProperty);
                }
            }

            // If referenced table/column are not specified, try to infer from navigation property
            if (relationship.NavigationProperty != null && relationship.ReferencedEntityType != null)
            {
                if (string.IsNullOrEmpty(relationship.ReferencedTableName))
                {
                    var referencedTableAttr = relationship.ReferencedEntityType.GetCustomAttribute<TableAttribute>();
                    relationship.ReferencedTableName = referencedTableAttr?.Name ?? relationship.ReferencedEntityType.Name;
                }

                if (string.IsNullOrEmpty(relationship.ReferencedColumnName))
                {
                    // Get the primary key of the referenced entity
                    var referencedMetadata = ExtractTableMetadata(relationship.ReferencedEntityType);
                    relationship.ReferencedColumnName = referencedMetadata.PrimaryKey.ColumnName;
                }
            }
        }
    }

    private static Type? GetEntityTypeFromNavigationProperty(PropertyInfo navigationProperty)
    {
        var propertyType = navigationProperty.PropertyType;

        // Handle single navigation property (one-to-many, many-to-one)
        if (propertyType.IsClass && propertyType != typeof(string))
        {
            return propertyType;
        }

        // Handle collection navigation property (one-to-many, many-to-many)
        if (propertyType.IsGenericType)
        {
            var genericArgs = propertyType.GetGenericArguments();
            if (genericArgs.Length == 1)
            {
                var elementType = genericArgs[0];
                if (elementType.IsClass && elementType != typeof(string))
                {
                    return elementType;
                }
            }
        }

        // Handle IEnumerable<T>
        if (propertyType.IsInterface && propertyType.IsGenericType)
        {
            var genericArgs = propertyType.GetGenericArguments();
            if (genericArgs.Length == 1)
            {
                var elementType = genericArgs[0];
                if (elementType.IsClass && elementType != typeof(string))
                {
                    return elementType;
                }
            }
        }

        return null;
    }

    private static bool IsNavigationProperty(PropertyInfo property)
    {
        var propertyType = property.PropertyType;
        
        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        // Skip if it's a primitive type, string, or value type
        if (underlyingType.IsPrimitive || underlyingType == typeof(string) || underlyingType.IsValueType)
            return false;

        // Check if it's a collection type
        if (propertyType.IsGenericType)
        {
            var genericTypeDefinition = propertyType.GetGenericTypeDefinition();
            if (genericTypeDefinition == typeof(ICollection<>) ||
                genericTypeDefinition == typeof(IList<>) ||
                genericTypeDefinition == typeof(List<>) ||
                genericTypeDefinition == typeof(IEnumerable<>))
            {
                return true;
            }
        }

        // Check if it implements IEnumerable (but not string)
        if (propertyType != typeof(string) && typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType))
        {
            return true;
        }

        // If it's a class type and doesn't have a Column attribute, treat it as a navigation property
        // Navigation properties are entity types that shouldn't be mapped as columns
        if (underlyingType.IsClass && underlyingType != typeof(string))
        {
            // If it has a Column attribute, it's explicitly mapped, so it's not a navigation property
            var hasColumnAttribute = property.GetCustomAttribute<ColumnAttribute>() != null;
            return !hasColumnAttribute;
        }

        return false;
    }

    private static SqlDataType InferSqlDataType(Type propertyType)
    {
        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        return underlyingType switch
        {
            _ when underlyingType == typeof(int) || underlyingType == typeof(long) => SqlDataType.Int,
            _ when underlyingType == typeof(decimal) => SqlDataType.Decimal,
            _ when underlyingType == typeof(float) || underlyingType == typeof(double) => SqlDataType.Float,
            _ when underlyingType == typeof(string) => SqlDataType.Varchar,
            _ when underlyingType == typeof(char) => SqlDataType.Char,
            _ when underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset) => SqlDataType.TimestampWithoutTimeZone,
            _ => SqlDataType.Varchar // Default fallback
        };
    }

    private static bool IsNonNullableValueType(Type type)
    {
        var underlyingType = Nullable.GetUnderlyingType(type);
        return underlyingType == null && type.IsValueType;
    }
}

