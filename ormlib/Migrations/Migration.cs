namespace OrmLib.Migrations;

/// <summary>
/// Represents a database migration with forward and backward operations.
/// </summary>
public sealed class Migration
{
    /// <summary>
    /// Gets the unique identifier for this migration.
    /// </summary>
    public string Id { get; }

    /// <summary>
    /// Gets the name/description of this migration.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the SQL statements to execute for forward migration (applying the migration).
    /// </summary>
    public IReadOnlyList<string> UpStatements { get; }

    /// <summary>
    /// Gets the SQL statements to execute for backward migration (rolling back the migration).
    /// </summary>
    public IReadOnlyList<string> DownStatements { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Migration"/> class.
    /// </summary>
    /// <param name="id">The unique identifier for this migration.</param>
    /// <param name="name">The name/description of this migration.</param>
    /// <param name="upStatements">The SQL statements for forward migration.</param>
    /// <param name="downStatements">The SQL statements for backward migration.</param>
    public Migration(
        string id,
        string name,
        IEnumerable<string> upStatements,
        IEnumerable<string> downStatements)
    {
        Id = id ?? throw new ArgumentNullException(nameof(id));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        UpStatements = upStatements?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(upStatements));
        DownStatements = downStatements?.ToList().AsReadOnly() ?? throw new ArgumentNullException(nameof(downStatements));
    }
}

