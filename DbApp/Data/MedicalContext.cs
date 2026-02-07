using DbApp.Models;
using OrmLib.Database;
using OrmLib.Metadata;

namespace DbApp.Data;

/// <summary>
/// Database context for the medical system application.
/// Manages all entity sets and provides database access.
/// </summary>
public class MedicalContext : DatabaseContext
{
    /// <summary>
    /// Gets the set of patients.
    /// </summary>
    public DbSet<Patient> Patients { get; }

    /// <summary>
    /// Gets the set of doctors.
    /// </summary>
    public DbSet<Doctor> Doctors { get; }

    /// <summary>
    /// Gets the set of disease histories.
    /// </summary>
    public DbSet<DiseaseHistory> DiseaseHistories { get; }

    /// <summary>
    /// Gets the set of medications.
    /// </summary>
    public DbSet<Medication> Medications { get; }

    /// <summary>
    /// Gets the set of examinations.
    /// </summary>
    public DbSet<Examination> Examinations { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="MedicalContext"/> class.
    /// </summary>
    /// <param name="connectionString">The PostgreSQL connection string.</param>
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


