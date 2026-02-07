using Npgsql;
using OrmLib.ChangeTracking;
using OrmLib.Metadata;
using OrmLib.Migrations;
using OrmLib.Sql;

namespace OrmLib.Database;

/// <summary>
/// Base class for database context. Manages database connection and provides access to entity sets.
/// </summary>
public abstract class DatabaseContext : IDisposable
{
    private readonly string _connectionString;
    private NpgsqlConnection? _connection;
    private bool _disposed = false;

    /// <summary>
    /// Gets the database connection.
    /// </summary>
    protected internal NpgsqlConnection Connection
    {
        get
        {
            if (_connection == null)
            {
                _connection = new NpgsqlConnection(_connectionString);
                _connection.Open();
            }
            else if (_connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }

            return _connection;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="DatabaseContext"/> class.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
    protected DatabaseContext(string connectionString)
    {
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    /// Gets all table metadata registered in this context.
    /// </summary>
    /// <returns>A collection of table metadata.</returns>
    protected abstract IEnumerable<TableMetadata> GetTableMetadata();

    /// <summary>
    /// Gets the change tracker for this context.
    /// </summary>
    public ChangeTracker ChangeTracker { get; } = new ChangeTracker();

    /// <summary>
    /// Gets the migration executor for this context.
    /// </summary>
    public MigrationExecutor Migrations => new MigrationExecutor(Connection);

    /// <summary>
    /// Generates a migration by comparing the current database schema with the entity metadata.
    /// </summary>
    /// <returns>A migration containing the differences, or null if schemas match.</returns>
    public Migration? GenerateMigration()
    {
        var currentSchema = SchemaReader.ReadSchema(Connection);
        var targetMetadata = GetTableMetadata();
        return MigrationGenerator.GenerateMigration(currentSchema, targetMetadata);
    }

    /// <summary>
    /// Detects changes in all tracked entities.
    /// </summary>
    public void DetectChanges()
    {
        ChangeTracker.DetectChanges();
    }

    /// <summary>
    /// Saves all changes made to tracked entities to the database.
    /// </summary>
    public void SaveChanges()
    {
        DetectChanges();

        foreach (var entry in ChangeTracker.Entries)
        {
            var metadata = entry.Metadata;
            var entityType = metadata.EntityType;

            switch (entry.State)
            {
                case EntityState.Added:
                    InsertEntity(entry.Entity, metadata);
                    entry.Reset();
                    break;

                case EntityState.Modified:
                    if (entry.ChangedColumns.Count > 0)
                    {
                        UpdateEntity(entry.Entity, metadata, entry.ChangedColumns);
                        entry.Reset();
                    }
                    break;

                case EntityState.Deleted:
                    DeleteEntity(entry.Entity, metadata);
                    ChangeTracker.Detach(entry.Entity, metadata);
                    break;

                case EntityState.Unchanged:
                    // No action needed
                    break;
            }
        }
    }

    private void InsertEntity(object entity, TableMetadata metadata)
    {
        var (sql, columns) = InsertSqlBuilder.BuildInsert(metadata);

        using var command = new NpgsqlCommand(sql, Connection);

        // Set parameters for columns
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var value = column.GetValue(entity);
            command.Parameters.AddWithValue($"@p{i + 1}", value ?? DBNull.Value);
        }

        // Execute INSERT
        if (metadata.PrimaryKey.IsAutoIncrement)
        {
            var newId = command.ExecuteScalar();
            if (newId != null && newId != DBNull.Value)
            {
                metadata.PrimaryKey.SetValue(entity, newId);
            }
        }
        else
        {
            command.ExecuteNonQuery();
        }
    }

    private void UpdateEntity(object entity, TableMetadata metadata, IEnumerable<ColumnMetadata> changedColumns)
    {
        var (sql, columns) = UpdateSqlBuilder.BuildUpdate(metadata, changedColumns);

        using var command = new NpgsqlCommand(sql, Connection);

        // Set parameters for changed columns
        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var value = column.GetValue(entity);
            command.Parameters.AddWithValue($"@p{i + 1}", value ?? DBNull.Value);
        }

        // Set parameter for primary key in WHERE clause
        var primaryKeyValue = metadata.PrimaryKey.GetValue(entity);
        command.Parameters.AddWithValue($"@p{columns.Count + 1}", primaryKeyValue ?? throw new InvalidOperationException("Primary key value cannot be null."));

        command.ExecuteNonQuery();
    }

    private void DeleteEntity(object entity, TableMetadata metadata)
    {
        var sql = DeleteSqlBuilder.BuildDelete(metadata);
        var primaryKeyValue = metadata.PrimaryKey.GetValue(entity);

        if (primaryKeyValue == null)
            throw new InvalidOperationException("Cannot delete entity with null primary key.");

        using var command = new NpgsqlCommand(sql, Connection);
        command.Parameters.AddWithValue("@p1", primaryKeyValue);

        command.ExecuteNonQuery();
    }

    /// <summary>
    /// Disposes the database context and closes the connection.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Disposes the database context and closes the connection.
    /// </summary>
    /// <param name="disposing">True if called from Dispose(), false if called from finalizer.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed && disposing)
        {
            _connection?.Close();
            _connection?.Dispose();
            _connection = null;
            _disposed = true;
        }
    }
}

