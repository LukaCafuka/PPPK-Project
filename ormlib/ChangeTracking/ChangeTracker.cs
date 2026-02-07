using OrmLib.Metadata;

namespace OrmLib.ChangeTracking;

/// <summary>
/// Tracks changes to entities and manages their state.
/// </summary>
public sealed class ChangeTracker
{
    private readonly Dictionary<(Type EntityType, object PrimaryKey), EntityEntry> _entries = new();

    /// <summary>
    /// Gets all tracked entity entries.
    /// </summary>
    public IEnumerable<EntityEntry> Entries => _entries.Values;

    /// <summary>
    /// Attaches an entity to the change tracker.
    /// </summary>
    /// <param name="entity">The entity to attach.</param>
    /// <param name="metadata">The table metadata for the entity.</param>
    /// <returns>The entity entry.</returns>
    /// <exception cref="InvalidOperationException">Thrown when an entity with the same primary key is already tracked.</exception>
    public EntityEntry Attach(object entity, TableMetadata metadata)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        var primaryKey = metadata.PrimaryKey.GetValue(entity);
        if (primaryKey == null)
            throw new InvalidOperationException("Cannot attach entity with null primary key.");

        var key = (entity.GetType(), primaryKey);

        if (_entries.ContainsKey(key))
        {
            throw new InvalidOperationException($"Entity with primary key {primaryKey} is already being tracked.");
        }

        var entry = new EntityEntry(entity, metadata);
        _entries[key] = entry;
        return entry;
    }

    /// <summary>
    /// Detaches an entity from the change tracker.
    /// </summary>
    /// <param name="entity">The entity to detach.</param>
    /// <param name="metadata">The table metadata for the entity.</param>
    /// <returns>True if the entity was detached, false if it wasn't tracked.</returns>
    public bool Detach(object entity, TableMetadata metadata)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        var primaryKey = metadata.PrimaryKey.GetValue(entity);
        if (primaryKey == null)
            return false;

        var key = (entity.GetType(), primaryKey);
        return _entries.Remove(key);
    }

    /// <summary>
    /// Gets the entry for a tracked entity.
    /// </summary>
    /// <param name="entity">The entity.</param>
    /// <param name="metadata">The table metadata for the entity.</param>
    /// <returns>The entity entry, or null if the entity is not tracked.</returns>
    public EntityEntry? GetEntry(object entity, TableMetadata metadata)
    {
        if (entity == null)
            throw new ArgumentNullException(nameof(entity));
        if (metadata == null)
            throw new ArgumentNullException(nameof(metadata));

        var primaryKey = metadata.PrimaryKey.GetValue(entity);
        if (primaryKey == null)
            return null;

        var key = (entity.GetType(), primaryKey);
        return _entries.TryGetValue(key, out var entry) ? entry : null;
    }

    /// <summary>
    /// Detects changes in all tracked entities.
    /// </summary>
    public void DetectChanges()
    {
        foreach (var entry in _entries.Values)
        {
            entry.DetectChanges();
        }
    }

    /// <summary>
    /// Clears all tracked entities.
    /// </summary>
    public void Clear()
    {
        _entries.Clear();
    }
}

