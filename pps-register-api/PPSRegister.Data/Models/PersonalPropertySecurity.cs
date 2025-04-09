using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CsvHelper.Configuration.Attributes;

namespace PPSRegister.Data.Models;


public class PersonalPropertySecurity
{
  [Key]
  public int Id { get; set; }

  [Required]
  [StringLength(35)]
  [Name("Grantor First Name")]
  public string GrantorFirstName { get; set; } = string.Empty;

  [StringLength(75)]
  [Name("Grantor Middle Names")]
  public string? GrantorMiddleNames { get; set; }

  [Required]
  [StringLength(35)]
  [Name("Grantor Last Name")]
  public string GrantorLastName { get; set; } = string.Empty;

  [Required]
  [StringLength(17)]
  [Name("VIN")]
  public string VIN { get; set; } = string.Empty;

  [Required]
  [Name("Registration start date")]
  public DateTime RegistrationStartDate { get; set; }

  [Required]
  [Name("Registration duration")]
  public string RegistrationDuration { get; set; } = string.Empty;

  [Required]
  [Name("SPG ACN")]
  [RegularExpression(@"^[\d\s,-]*(\d[\d\s,-]*){9}$", ErrorMessage = "ACN must contain exactly 9 digits")]
  public string SpgAcn { get; set; } = string.Empty;

  [Required]
  [StringLength(75)]
  [Name("SPG Organization Name")]
  public string SpgOrganizationName { get; set; } = string.Empty;

  [Required]
  public int ClientId { get; set; }
}