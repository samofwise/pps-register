using CsvHelper.Configuration.Attributes;

namespace PPSRegister.PPSUploader.Services;

public class PersonalPropertySecurityRecord
{
  [Name("Grantor First Name")]
  public string? GrantorFirstName { get; set; }

  [Name("Grantor Middle Names")]
  public string? GrantorMiddleNames { get; set; }

  [Name("Grantor Last Name")]
  public string? GrantorLastName { get; set; }

  [Name("VIN")]
  public string? VIN { get; set; }

  [Name("Registration start date")]
  public string? RegistrationStartDate { get; set; }

  [Name("Registration duration")]
  public string? RegistrationDuration { get; set; }

  [Name("SPG ACN")]
  public string? SpgAcn { get; set; }

  [Name("SPG Organization Name")]
  public string? SpgOrganizationName { get; set; }
}
