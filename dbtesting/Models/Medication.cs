using Dbtesting.Models;
using OrmLib.Attributes;

namespace Dbtesting.Models;

[Table("medication")]
public class Medication
{
    [Key]
    [AutoIncrement]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [ForeignKey("patient", "id")]
    [Column("patient_id")]
    public long PatientId { get; set; }

    [Required]
    [Column("medication_name", SqlDataType.Varchar)]
    public string MedicationName { get; set; } = string.Empty;

    [Required]
    [Column("dose", SqlDataType.Decimal)]
    public decimal Dose { get; set; }

    [Required]
    [Column("dose_unit", SqlDataType.Varchar)]
    public string DoseUnit { get; set; } = string.Empty;

    [Required]
    [Column("frequency", SqlDataType.Varchar)]
    public string Frequency { get; set; } = string.Empty;

    [Column("condition", SqlDataType.Text)]
    public string? Condition { get; set; }

    [Required]
    [Column("prescribed_date", SqlDataType.TimestampWithoutTimeZone)]
    public DateTime PrescribedDate { get; set; }

    // Navigation property (not mapped to database)
    public Patient? Patient { get; set; }
}