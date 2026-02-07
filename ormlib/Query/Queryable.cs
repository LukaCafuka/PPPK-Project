using System.Linq.Expressions;
using OrmLib.Database;
using OrmLib.Metadata;
using OrmLib.Sql;

namespace OrmLib.Query;

/// <summary>
/// Represents information about a navigation property to include.
/// </summary>
internal class IncludeInfo
{
    public string PropertyName { get; }
    public Type PropertyType { get; }

    public IncludeInfo(string propertyName, Type propertyType)
    {
        PropertyName = propertyName;
        PropertyType = propertyType;
    }
}

/// <summary>
/// Provides a queryable interface for building SQL queries with filtering and sorting.
/// </summary>
/// <typeparam name="T">The entity type.</typeparam>
public class Queryable<T> where T : class, new()
{
    private readonly DbSet<T> _dbSet;
    private readonly TableMetadata _metadata;
    private string? _whereClause;
    private readonly List<object?> _whereParameters = new();
    private readonly List<string> _orderByClauses = new();
    private readonly List<IncludeInfo> _includes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Queryable{T}"/> class.
    /// </summary>
    /// <param name="dbSet">The DbSet instance.</param>
    internal Queryable(DbSet<T> dbSet)
    {
        _dbSet = dbSet ?? throw new ArgumentNullException(nameof(dbSet));
        _metadata = dbSet.Metadata;
    }

    /// <summary>
    /// Filters the query using a predicate expression.
    /// </summary>
    /// <param name="predicate">The predicate expression.</param>
    /// <returns>The queryable instance for method chaining.</returns>
    public Queryable<T> Where(Expression<Func<T, bool>> predicate)
    {
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        var visitor = new SqlExpressionVisitor(_metadata);
        var (sql, parameters) = visitor.Translate(predicate.Body);

        _whereClause = sql;
        _whereParameters.Clear();
        _whereParameters.AddRange(parameters);

        return this;
    }

    /// <summary>
    /// Sorts the query in ascending order by the specified key selector.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keySelector">The key selector expression.</param>
    /// <returns>The queryable instance for method chaining.</returns>
    public Queryable<T> OrderBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        _orderByClauses.Clear();
        AddOrderByClause(keySelector, "ASC");
        return this;
    }

    /// <summary>
    /// Sorts the query in descending order by the specified key selector.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keySelector">The key selector expression.</param>
    /// <returns>The queryable instance for method chaining.</returns>
    public Queryable<T> OrderByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        _orderByClauses.Clear();
        AddOrderByClause(keySelector, "DESC");
        return this;
    }

    /// <summary>
    /// Performs a subsequent ordering in ascending order by the specified key selector.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keySelector">The key selector expression.</param>
    /// <returns>The queryable instance for method chaining.</returns>
    public Queryable<T> ThenBy<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        AddOrderByClause(keySelector, "ASC");
        return this;
    }

    /// <summary>
    /// Performs a subsequent ordering in descending order by the specified key selector.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <param name="keySelector">The key selector expression.</param>
    /// <returns>The queryable instance for method chaining.</returns>
    public Queryable<T> ThenByDescending<TKey>(Expression<Func<T, TKey>> keySelector)
    {
        if (keySelector == null)
            throw new ArgumentNullException(nameof(keySelector));

        AddOrderByClause(keySelector, "DESC");
        return this;
    }

    /// <summary>
    /// Specifies related entities to include in the query results (eager loading).
    /// </summary>
    /// <typeparam name="TProperty">The type of the navigation property.</typeparam>
    /// <param name="navigationPropertyPath">The navigation property to include.</param>
    /// <returns>The queryable instance for method chaining.</returns>
    public Queryable<T> Include<TProperty>(Expression<Func<T, TProperty>> navigationPropertyPath)
    {
        if (navigationPropertyPath == null)
            throw new ArgumentNullException(nameof(navigationPropertyPath));

        if (navigationPropertyPath.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            _includes.Add(new IncludeInfo(propertyName, typeof(TProperty)));
        }
        else
        {
            throw new NotSupportedException("Complex expressions in Include are not supported. Use simple property access.");
        }

        return this;
    }

    /// <summary>
    /// Executes the query and returns a list of entities.
    /// </summary>
    /// <returns>A list of entities matching the query.</returns>
    public List<T> ToList()
    {
        var orderByClause = _orderByClauses.Count > 0
            ? string.Join(", ", _orderByClauses)
            : null;

        return _dbSet.ToList(_whereClause, orderByClause, _whereParameters, _includes);
    }

    /// <summary>
    /// Executes the query and returns the first entity, or null if no entity is found.
    /// </summary>
    /// <returns>The first entity, or null if not found.</returns>
    public T? FirstOrDefault()
    {
        var list = ToList();
        return list.FirstOrDefault();
    }

    private void AddOrderByClause<TKey>(Expression<Func<T, TKey>> keySelector, string direction)
    {
        if (keySelector.Body is MemberExpression memberExpression)
        {
            var propertyName = memberExpression.Member.Name;
            var column = _metadata.GetColumnByPropertyName(propertyName);
            if (column != null)
            {
                _orderByClauses.Add($"\"{column.ColumnName}\" {direction}");
            }
            else
            {
                throw new InvalidOperationException($"Property {propertyName} not found in metadata.");
            }
        }
        else
        {
            throw new NotSupportedException("Complex expressions in OrderBy are not supported. Use simple property access.");
        }
    }
}

