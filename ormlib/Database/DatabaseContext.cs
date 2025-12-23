using Npgsql;
using OrmLib.Metadata;

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
    protected internal abstract IEnumerable<TableMetadata> GetTableMetadata();

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

