using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace PPSRegister.Data.Models;

//This might need to be an index against the clientId as well, depending if the VIN should be unique acroos the whole registry or just per client
[Index(nameof(VIN), IsUnique = true, Name = "IX_PersonalPropertySecurity_VIN")]
public class PersonalPropertySecurity
{
  [Key]
  public int Id { get; set; }

  [Required]
  [StringLength(35)]
  public string GrantorFirstName { get; set; } = string.Empty;

  [StringLength(75)]
  public string? GrantorMiddleNames { get; set; }

  [Required]
  [StringLength(35)]
  public string GrantorLastName { get; set; } = string.Empty;

  [Required]
  [StringLength(17, MinimumLength = 17)]
  public string VIN { get; set; } = string.Empty;

  [Required]
  public DateTime RegistrationStartDate { get; set; }

  [Required]
  public string RegistrationDuration { get; set; } = string.Empty;

  [Required]
  [RegularExpression(@"^[\d\s,-]*(\d[\d\s,-]*){9}$", ErrorMessage = "ACN must contain exactly 9 digits")]
  public string SpgAcn { get; set; } = string.Empty;

  [Required]
  [StringLength(75)]
  public string SpgOrganizationName { get; set; } = string.Empty;

  [Required]
  public int ClientId { get; set; }
}