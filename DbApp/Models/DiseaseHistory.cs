using OrmLib.Attributes;

namespace DbApp.Models;

[Table("disease_history")]
public class DiseaseHistory
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
    [Column("disease_name", SqlDataType.Varchar)]
    public string DiseaseName { get; set; } = string.Empty;

    [Required]
    [Column("start_date", SqlDataType.TimestampWithoutTimeZone)]
    public DateTime StartDate { get; set; }

    [Column("end_date", SqlDataType.TimestampWithoutTimeZone)]
    public DateTime? EndDate { get; set; }

    // Navigation property (not mapped to database)
    public Patient? Patient { get; set; }
}

