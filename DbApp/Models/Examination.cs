using OrmLib.Attributes;

namespace DbApp.Models;

[Table("examination")]
public class Examination
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
    [ForeignKey("doctor", "id")]
    [Column("doctor_id")]
    public long DoctorId { get; set; }

    [Required]
    [Column("examination_type", SqlDataType.Varchar)]
    public string ExaminationType { get; set; } = string.Empty;

    [Required]
    [Column("scheduled_date", SqlDataType.TimestampWithoutTimeZone)]
    public DateTime ScheduledDate { get; set; }

    [Column("status", SqlDataType.Varchar)]
    public string? Status { get; set; }

    // Navigation properties (not mapped to database)
    public Patient? Patient { get; set; }
    public Doctor? Doctor { get; set; }
}

