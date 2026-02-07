using OrmLib.Attributes;

namespace DbApp.Models;

[Table("doctor")]
public class Doctor
{
    [Key]
    [AutoIncrement]
    [Column("id")]
    public long Id { get; set; }

    [Required]
    [Column("first_name", SqlDataType.Varchar)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [Column("last_name", SqlDataType.Varchar)]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [Column("specialization", SqlDataType.Varchar)]
    public string Specialization { get; set; } = string.Empty;
}


