namespace DbApp.Configuration;

/// <summary>
/// Provides database configuration settings.
/// </summary>
public static class DatabaseConfig
{
    /// <summary>
    /// Gets the PostgreSQL connection string.
    /// </summary>
    public static string ConnectionString { get; } = 
        "Host=10.0.0.42;Port=5432;Username=algebra;Password=algebruh;Database=algebradb;";
}