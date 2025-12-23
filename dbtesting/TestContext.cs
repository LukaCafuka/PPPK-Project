using Dbtesting.Models;
using OrmLib.Database;
using OrmLib.Metadata;

namespace Dbtesting;

public class TestContext : DatabaseContext
{
    public DbSet<Patient> Patients { get; }

    public TestContext(string connectionString) : base(connectionString)
    {
        Patients = new DbSet<Patient>(this);
    }

    protected override IEnumerable<TableMetadata> GetTableMetadata()
    {
        yield return MetadataExtractor.ExtractTableMetadata<Patient>();
    }
}

