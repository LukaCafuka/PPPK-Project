using System.Data;
using Npgsql;
using OrmLib.Metadata;
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
    internal DbSet(DatabaseContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _metadata = MetadataExtractor.ExtractTableMetadata<T>();
    }

    /// <summary>
    /// Gets the table metadata for this entity set.
    /// </summary>
    public TableMetadata Metadata => _metadata;

    /// <summary>
    /// Finds an entity by its primary key.
    /// </summary>
    /// <param name="key">The primary key value.</param>
    /// <returns>The entity if found, otherwise null.</returns>
    public T? Find(object? key)
    {
        if (key == null)
            return null;

        var sql = SelectSqlBuilder.BuildSelectByPrimaryKey(_metadata);

        using var command = new NpgsqlCommand(sql, _context.Connection);
        command.Parameters.AddWithValue("$1", key);

        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            return MapToEntity(reader);
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
    /// <returns>A list of entities.</returns>
    public List<T> ToList(string? whereClause, string? orderByClause)
    {
        var sql = SelectSqlBuilder.BuildSelect(_metadata, whereClause, orderByClause);
        var entities = new List<T>();

        using var command = new NpgsqlCommand(sql, _context.Connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            var entity = MapToEntity(reader);
            if (entity != null)
            {
                entities.Add(entity);
            }
        }

        return entities;
    }

    /// <summary>
    /// Adds an entity to the context for insertion.
    /// </summary>
    /// <param name="entity">The entity to add.</param>
    public void Add(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var (sql, columns) = InsertSqlBuilder.BuildInsert(_metadata);

        using var command = new NpgsqlCommand(sql, _context.Connection);

        // Set parameters for columns
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var value = column.GetValue(entity);
            command.Parameters.AddWithValue($"${i + 1}", value ?? DBNull.Value);
        }

        // Execute INSERT
        if (_metadata.PrimaryKey.IsAutoIncrement)
        {
            // Use ExecuteScalar to get the returned primary key value
            var newId = command.ExecuteScalar();
            if (newId != null && newId != DBNull.Value)
            {
                _metadata.PrimaryKey.SetValue(entity, newId);
            }
        }
        else
        {
            command.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Updates an entity in the database.
    /// </summary>
    /// <param name="entity">The entity to update.</param>
    /// <param name="changedColumns">The columns that have been modified.</param>
    public void Update(T entity, IEnumerable<ColumnMetadata> changedColumns)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var (sql, columns) = UpdateSqlBuilder.BuildUpdate(_metadata, changedColumns);

        using var command = new NpgsqlCommand(sql, _context.Connection);

        // Set parameters for changed columns
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var value = column.GetValue(entity);
            command.Parameters.AddWithValue($"${i + 1}", value ?? DBNull.Value);
        }

        // Set parameter for primary key in WHERE clause
        var primaryKeyValue = _metadata.PrimaryKey.GetValue(entity);
        command.Parameters.AddWithValue($"${columns.Count + 1}", primaryKeyValue ?? throw new InvalidOperationException("Primary key value cannot be null."));

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Removes an entity from the database.
    /// </summary>
    /// <param name="entity">The entity to remove.</param>
    public void Remove(T entity)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));

        var sql = DeleteSqlBuilder.BuildDelete(_metadata);
        var primaryKeyValue = _metadata.PrimaryKey.GetValue(entity);

        if (primaryKeyValue == null)
            throw new InvalidOperationException("Cannot delete entity with null primary key.");

        using var command = new NpgsqlCommand(sql, _context.Connection);
        command.Parameters.AddWithValue("$1", primaryKeyValue);

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Removes an entity by its primary key.
    /// </summary>
    /// <param name="key">The primary key value.</param>
    public void Remove(object key)
    {
        if (key == null)
            throw new ArgumentNullException(nameof(key));

        var sql = DeleteSqlBuilder.BuildDelete(_metadata);

        using var command = new NpgsqlCommand(sql, _context.Connection);
        command.Parameters.AddWithValue("$1", key);

        command.ExecuteNonQuery();
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

