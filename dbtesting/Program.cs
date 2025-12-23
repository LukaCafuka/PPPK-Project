using Dbtesting.Models;
using Npgsql;
using OrmLib.Database;
using OrmLib.Metadata;
using OrmLib.Sql;

// Connection string - adjust as needed
const string connectionString = "Host=10.0.0.42;Port=5432;Username=algebra;Password=algebruh;Database=algebradb;";

Console.WriteLine("Testing ORM Library - Creating Patients Table");
Console.WriteLine("=============================================\n");

try
{
    // Extract metadata for Patient entity
    var patientMetadata = MetadataExtractor.ExtractTableMetadata<Patient>();
    
    Console.WriteLine($"Table Name: {patientMetadata.TableName}");
    Console.WriteLine($"Columns: {patientMetadata.Columns.Count}");
    Console.WriteLine("\nColumn Details:");
    foreach (var column in patientMetadata.Columns)
    {
        Console.WriteLine($"  - {column.ColumnName} ({column.DataType}) " +
                         $"PK: {column.IsPrimaryKey}, " +
                         $"Required: {column.IsRequired}, " +
                         $"Unique: {column.IsUnique}, " +
                         $"AutoIncrement: {column.IsAutoIncrement}");
    }

    // Generate CREATE TABLE SQL
    var createTableSql = SqlGenerator.GenerateCreateTable(patientMetadata);
    
    Console.WriteLine("\nGenerated SQL:");
    Console.WriteLine("--------------");
    Console.WriteLine(createTableSql);
    Console.WriteLine();

    // Connect to database and execute CREATE TABLE
    Console.WriteLine("Connecting to database...");
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("Connected successfully!\n");

    // Drop table if exists (for testing)
    var dropTableSql = SqlGenerator.GenerateDropTable(patientMetadata, ifExists: true);
    Console.WriteLine($"Executing: {dropTableSql}");
    await using (var dropCommand = new NpgsqlCommand(dropTableSql, connection))
    {
        await dropCommand.ExecuteNonQueryAsync();
    }
    Console.WriteLine("Dropped existing table (if any).\n");

    // Create table
    Console.WriteLine("Creating table...");
    await using var createCommand = new NpgsqlCommand(createTableSql, connection);
    await createCommand.ExecuteNonQueryAsync();
    Console.WriteLine("Table created successfully!\n");

    Console.WriteLine("✅ Test completed successfully!");
}
catch (Npgsql.NpgsqlException ex)
{
    Console.WriteLine($"❌ Database Error: {ex.Message}");
    Console.WriteLine("\nMake sure PostgreSQL is running and the connection string is correct.");
}
catch (Exception ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
    Console.WriteLine($"Stack Trace: {ex.StackTrace}");
}
