using System.Data;
using System.Linq;
using System.Reflection;
using Npgsql;
using OrmLib.ChangeTracking;
using OrmLib.Metadata;
using OrmLib.Query;
using OrmLib.Relationships;
using OrmLib.Sql;

namespace OrmLib.Database;

/// <summary>
/// Represents a database table and provides CRUD operations.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class DbSet<T> where T : class, new()
{
    private readonly DatabaseContext _context;
    private readonly TableMetadata _metadata;

    /// <summary>
    /// Initializes a new instance of the <see cref="DbSet{T}"/> class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public DbSet(DatabaseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _metadata = MetadataExtractor.ExtractTableMetadata<T>();
    }

    /// <summary>
    /// Gets the table metadata for this entity set.
    /// </summary>
    public TableMetadata Metadata => _metadata;

    /// <summary>
    /// Creates a queryable instance for building queries with filtering and sorting.
    /// </summary>
    /// <returns>A queryable instance.</returns>
    public Queryable<T> AsQueryable()
    {
        return new Queryable<T>(this);
    }

    /// <summary>
    /// Finds an entity by its primary key.
    /// The entity will be tracked with Unchanged state.
    /// </summary>
    /// <param name="key">The primary key value.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    public T? Find(object? key)
    {
        if (key == null)
            return null;

        var sql = SelectSqlBuilder.BuildSelectByPrimaryKey(_metadata);

        using var command = new NpgsqlCommand(sql, _context.Connection);
        command.Parameters.AddWithValue("@p1", key);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            var entity = MapToEntity(reader);
            if (entity != null)
            {
                // Check if entity is already tracked before attaching
                var existingEntry = _context.ChangeTracker.GetEntry(entity, _metadata);
                if (existingEntry == null)
                {
                    // Attach entity to change tracker with Unchanged state
                    _context.ChangeTracker.Attach(entity, _metadata);
                }
            }
            return entity;
        }

        return null;
    }

    /// <summary>
    /// Gets all entities from the table.
    /// </summary>
    /// <returns>A list of all entities.</returns>
    public List<T> ToList()
    {
        return ToList(null, null);
    }

    /// <summary>
    /// Gets all entities from the table with optional filtering and sorting.
    /// </summary>
    /// <param name="whereClause">Optional WHERE clause (without the WHERE keyword).</param>
    /// <param name="orderByClause">Optional ORDER BY clause (without the ORDER BY keyword).</param>
    /// <param name="parameters">Optional parameters for the WHERE clause.</param>
    /// <param name="includes">Optional navigation properties to include (eager loading).</param>
    /// <returns>A list of entities.</returns>
    public List<T> ToList(string? whereClause, string? orderByClause, IEnumerable<object?>? parameters = null, IEnumerable<object>? includes = null)
    {
        var sql = SelectSqlBuilder.BuildSelect(_metadata, whereClause, orderByClause);
        var entities = new List<T>();

        using var command = new NpgsqlCommand(sql, _context.Connection);
        
        // Add parameters if provided
        if (parameters != null)
        {
            int paramIndex = 1;
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue($"@p{paramIndex}", param ?? DBNull.Value);
                paramIndex++;
            }
        }

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var entity = MapToEntity(reader);
            if (entity != null)
            {
                // Check if entity is already tracked before attaching
                var existingEntry = _context.ChangeTracker.GetEntry(entity, _metadata);
                if (existingEntry == null)
                {
                    // Attach entities to change tracker with Unchanged state
                    _context.ChangeTracker.Attach(entity, _metadata);
                }
                // If already tracked, use the existing tracked entity
                entities.Add(entity);
            }
        }
        
        // Ensure reader is fully closed before loading navigation properties
        // This is important because PostgreSQL connections can only have one active command at a time
        reader.Close();

        // Load navigation properties if includes are specified
        if (includes != null && includes.Any())
        {
            LoadNavigationProperties(entities, includes);
        }

        return entities;
    }

    private void LoadNavigationProperties(List<T> entities, IEnumerable<object> includes)
    {
        foreach (var includeObj in includes)
        {
            // Use reflection to get property name and type from IncludeInfo
            var propertyName = includeObj.GetType().GetProperty("PropertyName")?.GetValue(includeObj) as string;
            var propertyType = includeObj.GetType().GetProperty("PropertyType")?.GetValue(includeObj) as Type;

            if (string.IsNullOrEmpty(propertyName) || propertyType == null)
                continue;

            var navigationProperty = _metadata.EntityType.GetProperty(propertyName);
            if (navigationProperty == null)
                continue;

            // Determine if it's a single entity or collection
            var isCollection = propertyType.IsGenericType && 
                (propertyType.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(IList<>) ||
                 propertyType.GetGenericTypeDefinition() == typeof(List<>) ||
                 typeof(System.Collections.IEnumerable).IsAssignableFrom(propertyType));

            if (isCollection)
            {
                // For collection navigation properties, find the relationship on the child entity
                // that points back to this entity
                var elementType = propertyType.GetGenericArguments()[0];
                var childMetadata = MetadataExtractor.ExtractTableMetadata(elementType);
                
                // Find the relationship on the child entity that references this entity
                // Check both ReferencedEntityType and ReferencedTableName
                var relationship = childMetadata.Relationships.FirstOrDefault(r =>
                    (r.ReferencedEntityType != null && r.ReferencedEntityType == _metadata.EntityType) ||
                    (!string.IsNullOrEmpty(r.ReferencedTableName) && 
                     r.ReferencedTableName.Equals(_metadata.TableName, StringComparison.OrdinalIgnoreCase)));

                if (relationship != null)
                {
                    // Ensure ReferencedEntityType is set
                    if (relationship.ReferencedEntityType == null)
                    {
                        relationship.ReferencedEntityType = _metadata.EntityType;
                    }
                    LoadCollectionNavigationProperty(entities, relationship, navigationProperty, childMetadata);
                }
            }
            else
            {
                // For single navigation properties, find the relationship on this entity
                var relationship = _metadata.Relationships.FirstOrDefault(r => 
                    r.NavigationProperty?.Name == propertyName);

                if (relationship != null)
                {
                    LoadSingleNavigationProperty(entities, relationship, navigationProperty);
                }
            }
        }
    }

    private void LoadSingleNavigationProperty(List<T> entities, RelationshipMetadata relationship, PropertyInfo navigationProperty)
    {
        if (relationship.ReferencedEntityType == null)
            return;

        var referencedMetadata = MetadataExtractor.ExtractTableMetadata(relationship.ReferencedEntityType);

        foreach (var entity in entities)
        {
            var relatedEntity = NavigationPropertyLoader.LoadSingle(
                entity,
                relationship,
                referencedMetadata,
                _context.Connection);

            if (relatedEntity != null)
            {
                navigationProperty.SetValue(entity, relatedEntity);
            }
        }
    }

    private void LoadCollectionNavigationProperty(List<T> entities, RelationshipMetadata relationship, PropertyInfo navigationProperty, TableMetadata childMetadata)
    {
        foreach (var entity in entities)
        {
            var relatedEntities = NavigationPropertyLoader.LoadCollection(
                entity,
                relationship,
                childMetadata,
                _metadata,
                _context.Connection);

            // Create a collection instance and populate it
            var collectionType = navigationProperty.PropertyType;
            object? collection = null;

            if (collectionType.IsGenericType)
            {
                var genericType = collectionType.GetGenericTypeDefinition();
                if (genericType == typeof(List<>))
                {
                    var elementType = collectionType.GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    collection = Activator.CreateInstance(listType);
                    
                    if (collection != null)
                    {
                        var addMethod = listType.GetMethod("Add");
                        foreach (var relatedEntity in relatedEntities)
                        {
                            addMethod?.Invoke(collection, new[] { relatedEntity });
                        }
                    }
                }
                else if (genericType == typeof(ICollection<>) || genericType == typeof(IList<>))
                {
                    // Create a List<T> and assign it
                    var elementType = collectionType.GetGenericArguments()[0];
                    var listType = typeof(List<>).MakeGenericType(elementType);
                    collection = Activator.CreateInstance(listType);
                    
                    if (collection != null)
                    {
                        var addMethod = listType.GetMethod("Add");
                        foreach (var relatedEntity in relatedEntities)
                        {
                            addMethod?.Invoke(collection, new[] { relatedEntity });
                        }
                    }
                }
            }

            if (collection != null)
            {
                navigationProperty.SetValue(entity, collection);
            }
        }
    }

    /// <summary>
    /// Adds an entity to the context for insertion.
    /// The entity will be inserted when SaveChanges() is called.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void Add(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        // For new entities (Id = 0), check if there's already a tracked entity with Id = 0
        // If so, detach it first (it's likely a stale tracked entity)
        var primaryKeyValue = _metadata.PrimaryKey.GetValue(entity);
        if (primaryKeyValue != null)
        {
            var existingEntry = _context.ChangeTracker.GetEntry(entity, _metadata);
            if (existingEntry != null)
            {
                // If it's the same instance, just mark it as Added
                if (ReferenceEquals(existingEntry.Entity, entity))
                {
                    existingEntry.State = EntityState.Added;
                    return;
                }
                // Otherwise, detach the old one (it's a different instance with the same key)
                _context.ChangeTracker.Detach(existingEntry.Entity, _metadata);
            }
        }

        var entry = _context.ChangeTracker.Attach(entity, _metadata);
        entry.State = EntityState.Added;
    }

    /// <summary>
    /// Marks an entity for update.
    /// The entity will be updated when SaveChanges() is called.
    /// Changes are automatically detected by comparing current values with original values.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    public void Update(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var entry = _context.ChangeTracker.GetEntry(entity, _metadata);
        if (entry == null)
        {
            // If not tracked, attach it
            entry = _context.ChangeTracker.Attach(entity, _metadata);
        }

        // Detect changes will be called by SaveChanges()
        // Entity will be marked as Modified if changes are detected
    }

    /// <summary>
    /// Marks an entity for deletion.
    /// The entity will be deleted when SaveChanges() is called.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void Remove(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var entry = _context.ChangeTracker.GetEntry(entity, _metadata);
        if (entry == null)
        {
            // If not tracked, attach it first
            entry = _context.ChangeTracker.Attach(entity, _metadata);
        }

        entry.State = EntityState.Deleted;
    }

    /// <summary>
    /// Marks an entity for deletion by its primary key.
    /// The entity will be deleted when SaveChanges() is called.
    /// </summary>
    /// <param name="key">The primary key value.</param>
    public void Remove(object key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var entity = Find(key);
        if (entity == null)
            throw new InvalidOperationException($"Entity with primary key {key} not found.");

        Remove(entity);
    }

    private T MapToEntity(NpgsqlDataReader reader)
    {
        var entity = new T();

        foreach (var column in _metadata.Columns)
        {
            try
            {
                var columnName = column.ColumnName;
                var value = reader[columnName];

                if (value != DBNull.Value)
                {
                    // Handle nullable types
                    var propertyType = column.PropertyType;
                    var underlyingType = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

                    object? convertedValue = null;
                    if (underlyingType == typeof(DateTime) || underlyingType == typeof(DateTimeOffset))
                    {
                        convertedValue = Convert.ToDateTime(value);
                    }
                    else if (underlyingType.IsEnum)
                    {
                        convertedValue = Enum.ToObject(underlyingType, value);
                    }
                    else
                    {
                        convertedValue = Convert.ChangeType(value, underlyingType);
                    }

                    column.SetValue(entity, convertedValue);
                }
                else
                {
                    column.SetValue(entity, null);
                }
            }
            catch (Exception ex)
            {
                // Skip columns that don't exist in the result set or can't be mapped
                // This can happen with JOIN queries or missing columns
                System.Diagnostics.Debug.WriteLine($"Failed to map column {column.ColumnName}: {ex.Message}");
            }
        }

        return entity;
    }
}

