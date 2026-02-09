using DbApp.Models;
using OrmLib.Database;
using OrmLib.Metadata;

namespace DbApp.Data;

public class MedicalContext : DatabaseContext
{
    public DbSet<Patient> Patients { get; }

    public DbSet<Doctor> Doctors { get; }

    public DbSet<DiseaseHistory> DiseaseHistories { get; }

    public DbSet<Medication> Medications { get; }

    public DbSet<Examination> Examinations { get; }

    public MedicalContext(string connectionString) : base(connectionString)
    {
        Patients = new DbSet<Patient>(this);
        Doctors = new DbSet<Doctor>(this);
        DiseaseHistories = new DbSet<DiseaseHistory>(this);
        Medications = new DbSet<Medication>(this);
        Examinations = new DbSet<Examination>(this);
    }

    /// <summary>
    /// Gets all table metadata registered in this context.
    /// </summary>
    /// <returns>A collection of table metadata for all entities.</returns>
    protected override IEnumerable<TableMetadata> GetTableMetadata()
    {
        yield return MetadataExtractor.ExtractTableMetadata<Patient>();
        yield return MetadataExtractor.ExtractTableMetadata<Doctor>();
        yield return MetadataExtractor.ExtractTableMetadata<DiseaseHistory>();
        yield return MetadataExtractor.ExtractTableMetadata<Medication>();
        yield return MetadataExtractor.ExtractTableMetadata<Examination>();
    }
}


