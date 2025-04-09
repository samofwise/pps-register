using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PPSRegister.Data.Models;

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
  [StringLength(17)]
  public string VIN { get; set; } = string.Empty;

  [Required]
  public DateTime RegistrationStartDate { get; set; }

  [Required]
  [Range(1, 3, ErrorMessage = "Invalid Registration Duration")]
  public int RegistrationDuration { get; set; }

  [Required]
  [StringLength(9)]
  public string SpgAcn { get; set; } = string.Empty;

  [Required]
  [StringLength(75)]
  public string SpgOrganizationName { get; set; } = string.Empty;

  public DateTime DateRegistered { get; set; } = DateTime.Now;

  [Required]
  [ForeignKey(nameof(Client))]
  public int ClientId { get; set; }
  public Client Client { get; set; } = null!;
}