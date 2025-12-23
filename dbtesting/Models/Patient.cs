using OrmLib.Attributes;

namespace Dbtesting.Models;

[Table("patient")]
public class Patient
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
    [Unique]
    [Column("oib", SqlDataType.Char)]
    public string Oib { get; set; } = string.Empty;

    [Required]
    [Column("date_of_birth", SqlDataType.TimestampWithoutTimeZone)]
    public DateTime DateOfBirth { get; set; }

    [Required]
    [Column("gender", SqlDataType.Char)]
    public string Gender { get; set; } = string.Empty;

    [Column("residence_address", SqlDataType.Text)]
    public string? ResidenceAddress { get; set; }

    [Column("permanent_address", SqlDataType.Text)]
    public string? PermanentAddress { get; set; }
}

