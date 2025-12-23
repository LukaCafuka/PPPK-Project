using Dbtesting;
using Dbtesting.Models;
using Npgsql;
using OrmLib.Metadata;
using OrmLib.Sql;

// Connection string
const string connectionString = "Host=10.0.0.42;Port=5432;Username=algebra;Password=algebruh;Database=algebradb;";

if (args.Length == 0)
{
    PrintUsage();
    return;
}

var command = args[0].ToLower();

try
{
    switch (command)
    {
        case "create-table":
            await CreateTableAsync();
            break;
        case "drop-table":
            await DropTableAsync();
            break;
        case "insert":
            await InsertPatientAsync(args);
            break;
        case "find":
            await FindPatientAsync(args);
            break;
        case "list":
            await ListPatientsAsync(args);
            break;
        case "update":
            await UpdatePatientAsync(args);
            break;
        case "delete":
            await DeletePatientAsync(args);
            break;
        case "metadata":
            ShowMetadata();
            break;
        default:
            Console.WriteLine($"ERROR: Unknown command: {command}");
            PrintUsage();
            break;
    }
}
catch (Npgsql.NpgsqlException ex)
{
    Console.WriteLine($"ERROR: Database Error: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
    if (args.Length > 0 && args[0].ToLower() == "debug")
    {
        Console.WriteLine($"Stack Trace: {ex.StackTrace}");
    }
}

static void PrintUsage()
{
    Console.WriteLine("ORM Library Test Console");
    Console.WriteLine("========================");
    Console.WriteLine();
    Console.WriteLine("Usage: dotnet run -- <command> [arguments]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  create-table              Create the patient table");
    Console.WriteLine("  drop-table                Drop the patient table");
    Console.WriteLine("  insert <first> <last> <oib> <dob> <gender> [residence] [permanent]");
    Console.WriteLine("                            Insert a new patient");
    Console.WriteLine("                            Example: insert John Doe 12345678901 1990-01-01 M");
    Console.WriteLine("  find <id>                 Find a patient by ID");
    Console.WriteLine("  list [order-by]           List all patients (optionally ordered)");
    Console.WriteLine("                            Example: list \"last_name ASC\"");
    Console.WriteLine("  update <id> <field> <value>");
    Console.WriteLine("                            Update a patient field");
    Console.WriteLine("                            Example: update 1 first_name Jane");
    Console.WriteLine("  delete <id>               Delete a patient by ID");
    Console.WriteLine("  metadata                  Show table metadata");
    Console.WriteLine();
}

static async Task CreateTableAsync()
{
    Console.WriteLine("Creating Patient Table");
    Console.WriteLine("======================");
    
    var patientMetadata = MetadataExtractor.ExtractTableMetadata<Patient>();
    var createTableSql = SqlGenerator.GenerateCreateTable(patientMetadata);
    
    Console.WriteLine("\nGenerated SQL:");
    Console.WriteLine("--------------");
    Console.WriteLine(createTableSql);
    Console.WriteLine();
    
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    Console.WriteLine("Connected to database.\n");
    
    await using var command = new NpgsqlCommand(createTableSql, connection);
    await command.ExecuteNonQueryAsync();
    
    Console.WriteLine("SUCCESS: Table created successfully!");
}

static async Task DropTableAsync()
{
    Console.WriteLine("Dropping Patient Table");
    Console.WriteLine("=======================");
    
    var patientMetadata = MetadataExtractor.ExtractTableMetadata<Patient>();
    var dropTableSql = SqlGenerator.GenerateDropTable(patientMetadata, ifExists: true);
    
    Console.WriteLine($"Executing: {dropTableSql}\n");
    
    await using var connection = new NpgsqlConnection(connectionString);
    await connection.OpenAsync();
    
    await using var command = new NpgsqlCommand(dropTableSql, connection);
    await command.ExecuteNonQueryAsync();
    
    Console.WriteLine("SUCCESS: Table dropped successfully!");
}

static async Task InsertPatientAsync(string[] args)
{
    if (args.Length < 6)
    {
        Console.WriteLine("ERROR: Usage: insert <first> <last> <oib> <dob> <gender> [residence] [permanent]");
        Console.WriteLine("   Example: insert John Doe 12345678901 1990-01-01 M \"123 Main St\" \"123 Main St\"");
        return;
    }
    
    var patient = new Patient
    {
        FirstName = args[1],
        LastName = args[2],
        Oib = args[3],
        DateOfBirth = DateTime.Parse(args[4]),
        Gender = args[5],
        ResidenceAddress = args.Length > 6 ? args[6] : null,
        PermanentAddress = args.Length > 7 ? args[7] : null
    };
    
    Console.WriteLine("Inserting Patient");
    Console.WriteLine("=================");
    Console.WriteLine($"First Name: {patient.FirstName}");
    Console.WriteLine($"Last Name: {patient.LastName}");
    Console.WriteLine($"OIB: {patient.Oib}");
    Console.WriteLine($"Date of Birth: {patient.DateOfBirth:yyyy-MM-dd}");
    Console.WriteLine($"Gender: {patient.Gender}");
    Console.WriteLine();
    
    using var context = new TestContext(connectionString);
    context.Patients.Add(patient);
    
    Console.WriteLine($"SUCCESS: Patient inserted successfully! ID: {patient.Id}");
}

static async Task FindPatientAsync(string[] args)
{
    if (args.Length < 2 || !long.TryParse(args[1], out var id))
    {
        Console.WriteLine("ERROR: Usage: find <id>");
        return;
    }
    
    Console.WriteLine($"Finding Patient with ID: {id}");
    Console.WriteLine("==============================");
    
    using var context = new TestContext(connectionString);
    var patient = context.Patients.Find(id);
    
    if (patient == null)
    {
        Console.WriteLine("ERROR: Patient not found.");
        return;
    }
    
    PrintPatient(patient);
}

static async Task ListPatientsAsync(string[] args)
{
    Console.WriteLine("Listing All Patients");
    Console.WriteLine("====================");
    
    string? orderBy = args.Length > 1 ? string.Join(" ", args.Skip(1)) : null;
    
    using var context = new TestContext(connectionString);
    var patients = context.Patients.ToList(null, orderBy);
    
    if (patients.Count == 0)
    {
        Console.WriteLine("No patients found.");
        return;
    }
    
    Console.WriteLine($"Found {patients.Count} patient(s):\n");
    foreach (var patient in patients)
    {
        PrintPatient(patient);
        Console.WriteLine();
    }
}

static async Task UpdatePatientAsync(string[] args)
{
    if (args.Length < 4 || !long.TryParse(args[1], out var id))
    {
        Console.WriteLine("ERROR: Usage: update <id> <field> <value>");
        Console.WriteLine("   Example: update 1 first_name Jane");
        return;
    }
    
    var fieldName = args[2].ToLower();
    var value = args[3];
    
    Console.WriteLine($"Updating Patient ID: {id}");
    Console.WriteLine("========================");
    
    using var context = new TestContext(connectionString);
    var patient = context.Patients.Find(id);
    
    if (patient == null)
    {
        Console.WriteLine("ERROR: Patient not found.");
        return;
    }
    
    var metadata = context.Patients.Metadata;
    ColumnMetadata? column = null;
    
    // Find column by property name (case-insensitive)
    foreach (var col in metadata.Columns)
    {
        if (col.PropertyName.Equals(fieldName, StringComparison.OrdinalIgnoreCase))
        {
            column = col;
            break;
        }
    }
    
    if (column == null || column.IsPrimaryKey)
    {
        Console.WriteLine($"ERROR: Invalid field: {fieldName} (cannot update primary key)");
        return;
    }
    
    // Set the value
    object? convertedValue = value;
    if (column.PropertyType == typeof(DateTime) || column.PropertyType == typeof(DateTime?))
    {
        convertedValue = DateTime.Parse(value);
    }
    else if (column.PropertyType == typeof(long) || column.PropertyType == typeof(long?))
    {
        convertedValue = long.Parse(value);
    }
    else if (column.PropertyType == typeof(int) || column.PropertyType == typeof(int?))
    {
        convertedValue = int.Parse(value);
    }
    
    column.SetValue(patient, convertedValue);
    
    // Update
    context.Patients.Update(patient, new[] { column });
    
    Console.WriteLine($"SUCCESS: Patient updated successfully!");
    PrintPatient(patient);
}

static async Task DeletePatientAsync(string[] args)
{
    if (args.Length < 2 || !long.TryParse(args[1], out var id))
    {
        Console.WriteLine("ERROR: Usage: delete <id>");
        return;
    }
    
    Console.WriteLine($"Deleting Patient with ID: {id}");
    Console.WriteLine("===============================");
    
    using var context = new TestContext(connectionString);
    var patient = context.Patients.Find(id);
    
    if (patient == null)
    {
        Console.WriteLine("ERROR: Patient not found.");
        return;
    }
    
    context.Patients.Remove(id);
    Console.WriteLine("SUCCESS: Patient deleted successfully!");
}

static void ShowMetadata()
{
    Console.WriteLine("Patient Table Metadata");
    Console.WriteLine("======================");
    
    var metadata = MetadataExtractor.ExtractTableMetadata<Patient>();
    
    Console.WriteLine($"\nTable Name: {metadata.TableName}");
    Console.WriteLine($"Entity Type: {metadata.EntityType.Name}");
    Console.WriteLine($"Primary Key: {metadata.PrimaryKey.ColumnName}");
    Console.WriteLine($"Total Columns: {metadata.Columns.Count}");
    Console.WriteLine($"Foreign Keys: {metadata.ForeignKeyColumns.Count()}");
    
    Console.WriteLine("\nColumns:");
    Console.WriteLine("--------");
    foreach (var column in metadata.Columns)
    {
        Console.WriteLine($"  {column.ColumnName}");
        Console.WriteLine($"    Property: {column.PropertyName}");
        Console.WriteLine($"    Type: {column.DataType} ({column.PropertyType.Name})");
        Console.WriteLine($"    Primary Key: {column.IsPrimaryKey}");
        Console.WriteLine($"    Required: {column.IsRequired}");
        Console.WriteLine($"    Unique: {column.IsUnique}");
        Console.WriteLine($"    Auto Increment: {column.IsAutoIncrement}");
        if (column.Length.HasValue)
            Console.WriteLine($"    Length: {column.Length}");
        if (column.DefaultValue != null)
            Console.WriteLine($"    Default: {column.DefaultValue}");
        Console.WriteLine();
    }
    
    if (metadata.Relationships.Count > 0)
    {
        Console.WriteLine("Relationships:");
        Console.WriteLine("-------------");
        foreach (var rel in metadata.Relationships)
        {
            Console.WriteLine($"  {rel.ForeignKeyProperty.Name} -> {rel.ReferencedTableName}.{rel.ReferencedColumnName}");
        }
    }
}

static void PrintPatient(Patient patient)
{
    Console.WriteLine($"ID: {patient.Id}");
    Console.WriteLine($"First Name: {patient.FirstName}");
    Console.WriteLine($"Last Name: {patient.LastName}");
    Console.WriteLine($"OIB: {patient.Oib}");
    Console.WriteLine($"Date of Birth: {patient.DateOfBirth:yyyy-MM-dd}");
    Console.WriteLine($"Gender: {patient.Gender}");
    if (!string.IsNullOrEmpty(patient.ResidenceAddress))
        Console.WriteLine($"Residence Address: {patient.ResidenceAddress}");
    if (!string.IsNullOrEmpty(patient.PermanentAddress))
        Console.WriteLine($"Permanent Address: {patient.PermanentAddress}");
}
