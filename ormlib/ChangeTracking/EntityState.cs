namespace OrmLib.ChangeTracking;

/// <summary>
/// Represents the state of an entity in the change tracker.
/// </summary>
public enum EntityState
{
    /// <summary>
    /// The entity is being tracked but has not been modified.
    /// </summary>
    Unchanged,

    /// <summary>
    /// The entity is being tracked and will be inserted into the database.
    /// </summary>
    Added,

    /// <summary>
    /// The entity is being tracked and has been modified.
    /// </summary>
    Modified,

    /// <summary>
    /// The entity is being tracked and will be deleted from the database.
    /// </summary>
    Deleted
}

