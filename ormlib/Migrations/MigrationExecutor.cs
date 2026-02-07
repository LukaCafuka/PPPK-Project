using Npgsql;

namespace OrmLib.Migrations;

/// <summary>
/// Executes migrations forward and backward.
/// </summary>
public sealed class MigrationExecutor
{
    private readonly NpgsqlConnection _connection;
    private const string MigrationHistoryTableName = "__migrations_history";

    /// <summary>
    /// Initializes a new instance of the <see cref="MigrationExecutor"/> class.
    /// </summary>
    /// <param name="connection">The database connection.</param>
    public MigrationExecutor(NpgsqlConnection connection)
    {
        _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        EnsureMigrationHistoryTable();
    }

    /// <summary>
    /// Executes a migration forward (applies the migration).
    /// </summary>
    /// <param name="migration">The migration to execute.</param>
    public void ExecuteUp(Migration migration)
    {
        if (migration == null)
            throw new ArgumentNullException(nameof(migration));

        // Check if migration already applied
        if (IsMigrationApplied(migration.Id))
        {
            throw new InvalidOperationException($"Migration {migration.Id} has already been applied.");
        }

        using var transaction = _connection.BeginTransaction();
        try
        {
            foreach (var statement in migration.UpStatements)
            {
                if (!string.IsNullOrWhiteSpace(statement) && !statement.TrimStart().StartsWith("--"))
                {
                    using var command = new NpgsqlCommand(statement, _connection, transaction);
                    command.ExecuteNonQuery();
                }
            }

            // Record migration in history
            RecordMigration(migration.Id, migration.Name, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Executes a migration backward (rolls back the migration).
    /// </summary>
    /// <param name="migration">The migration to roll back.</param>
    public void ExecuteDown(Migration migration)
    {
        if (migration == null)
            throw new ArgumentNullException(nameof(migration));

        // Check if migration is applied
        if (!IsMigrationApplied(migration.Id))
        {
            throw new InvalidOperationException($"Migration {migration.Id} has not been applied.");
        }

        using var transaction = _connection.BeginTransaction();
        try
        {
            // Execute down statements in reverse order
            for (int i = migration.DownStatements.Count - 1; i >= 0; i--)
            {
                var statement = migration.DownStatements[i];
                if (!string.IsNullOrWhiteSpace(statement) && !statement.TrimStart().StartsWith("--"))
                {
                    using var command = new NpgsqlCommand(statement, _connection, transaction);
                    command.ExecuteNonQuery();
                }
            }

            // Remove migration from history
            RemoveMigration(migration.Id, transaction);

            transaction.Commit();
        }
        catch
        {
            transaction.Rollback();
            throw;
        }
    }

    /// <summary>
    /// Gets all applied migrations.
    /// </summary>
    /// <returns>A list of applied migration IDs.</returns>
    public List<string> GetAppliedMigrations()
    {
        var migrations = new List<string>();

        var query = $"SELECT migration_id FROM \"{MigrationHistoryTableName}\" ORDER BY applied_at";
        using var command = new NpgsqlCommand(query, _connection);
        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            migrations.Add(reader.GetString(0));
        }

        return migrations;
    }

    /// <summary>
    /// Checks if a migration has been applied.
    /// </summary>
    /// <param name="migrationId">The migration ID.</param>
    /// <returns>True if the migration has been applied, false otherwise.</returns>
    public bool IsMigrationApplied(string migrationId)
    {
        var query = $"SELECT COUNT(*) FROM \"{MigrationHistoryTableName}\" WHERE migration_id = @id";
        using var command = new NpgsqlCommand(query, _connection);
        command.Parameters.AddWithValue("@id", migrationId);

        var count = Convert.ToInt32(command.ExecuteScalar());
        return count > 0;
    }

    private void EnsureMigrationHistoryTable()
    {
        var createTableSql = $@"
            CREATE TABLE IF NOT EXISTS ""{MigrationHistoryTableName}"" (
                migration_id VARCHAR(255) PRIMARY KEY,
                migration_name VARCHAR(500) NOT NULL,
                applied_at TIMESTAMP WITHOUT TIME ZONE NOT NULL DEFAULT CURRENT_TIMESTAMP
            );";

        using var command = new NpgsqlCommand(createTableSql, _connection);
        command.ExecuteNonQuery();
    }

    private void RecordMigration(string migrationId, string migrationName, NpgsqlTransaction transaction)
    {
        var insertSql = $@"
            INSERT INTO ""{MigrationHistoryTableName}"" (migration_id, migration_name, applied_at)
            VALUES (@id, @name, CURRENT_TIMESTAMP);";

        using var command = new NpgsqlCommand(insertSql, _connection, transaction);
        command.Parameters.AddWithValue("@id", migrationId);
        command.Parameters.AddWithValue("@name", migrationName);
        command.ExecuteNonQuery();
    }

    private void RemoveMigration(string migrationId, NpgsqlTransaction transaction)
    {
        var deleteSql = $"DELETE FROM \"{MigrationHistoryTableName}\" WHERE migration_id = @id";
        using var command = new NpgsqlCommand(deleteSql, _connection, transaction);
        command.Parameters.AddWithValue("@id", migrationId);
        command.ExecuteNonQuery();
    }
}

