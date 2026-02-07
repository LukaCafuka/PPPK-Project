using System.Collections.ObjectModel;
using OrmLib.Metadata;

namespace OrmLib.ChangeTracking;

/// <summary>
/// Represents a tracked entity and its state information.
/// </summary>
public sealed class EntityEntry
{
    private readonly Dictionary<ColumnMetadata, object?> _originalValues = new();
    private readonly List<ColumnMetadata> _changedColumns = new();

    /// <summary>
    /// Gets the tracked entity.
    /// </summary>
    public object Entity { get; }

    /// <summary>
    /// Gets or sets the state of the entity.
    /// </summary>
    public EntityState State { get; set; }

    /// <summary>
    /// Gets the table metadata for the entity.
    /// </summary>
    public TableMetadata Metadata { get; }

    /// <summary>
    /// Gets the columns that have been modified.
    /// </summary>
    public ReadOnlyCollection<ColumnMetadata> ChangedColumns { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityEntry"/> class.
    /// </summary>
    /// <param name="entity">The entity to track.</param>
    /// <param name="metadata">The table metadata for the entity.</param>
    internal EntityEntry(object entity, TableMetadata metadata)
    {
        Entity = entity ?? throw new ArgumentNullException(nameof(entity));
        Metadata = metadata ?? throw new ArgumentNullException(nameof(metadata));
        State = EntityState.Unchanged;
        ChangedColumns = new ReadOnlyCollection<ColumnMetadata>(_changedColumns);

        // Store original values
        StoreOriginalValues();
    }

    /// <summary>
    /// Detects changes by comparing current values with original values.
    /// </summary>
    public void DetectChanges()
    {
        // Don't detect changes if entity is already marked as Added or Deleted
        if (State == EntityState.Added || State == EntityState.Deleted)
            return;

        _changedColumns.Clear();

        foreach (var column in Metadata.Columns)
        {
            var originalValue = _originalValues[column];
            var currentValue = column.GetValue(Entity);

            if (!Equals(originalValue, currentValue))
            {
                _changedColumns.Add(column);
                State = EntityState.Modified;
            }
        }

        // If no changes detected, reset to Unchanged
        if (_changedColumns.Count == 0 && State == EntityState.Modified)
        {
            State = EntityState.Unchanged;
        }
    }

    /// <summary>
    /// Resets the entry to Unchanged state and updates original values.
    /// </summary>
    internal void Reset()
    {
        State = EntityState.Unchanged;
        _changedColumns.Clear();
        StoreOriginalValues();
    }

    /// <summary>
    /// Gets the original value of a column.
    /// </summary>
    /// <param name="column">The column metadata.</param>
    /// <returns>The original value, or null if not found.</returns>
    public object? GetOriginalValue(ColumnMetadata column)
    {
        return _originalValues.TryGetValue(column, out var value) ? value : null;
    }

    private void StoreOriginalValues()
    {
        _originalValues.Clear();
        foreach (var column in Metadata.Columns)
        {
            var value = column.GetValue(Entity);
            _originalValues[column] = value;
        }
    }
}

