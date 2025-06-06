using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using PPSRegister.Data;
using PPSRegister.Data.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace PPSRegister.PPSUploader.Services;

public interface IPPSUploadProcessingService
{
  Task ProcessUpload(PPSUploadMessage? uploadMessage);
}

public class PPSUploadProcessingService(IDbContextFactory<PPSRegisterDbContext> _contextFactory, ILogger<PPSUploadProcessingService> _logger) : IPPSUploadProcessingService
{
  public async Task ProcessUpload(PPSUploadMessage? uploadMessage)
  {
    if (uploadMessage == null) return;

    using var context = await _contextFactory.CreateDbContextAsync();

    var config = new CsvConfiguration(CultureInfo.InvariantCulture)
    {
      HasHeaderRecord = true,
      MissingFieldFound = null,
      HeaderValidated = null
    };

    using var reader = new StringReader(uploadMessage.Data);
    using var csv = new CsvReader(reader, config);

    var records = csv.GetRecords<PersonalPropertySecurityRecord>().ToList();

    var uploadRecord = context.PersonalPropertySecurityUploads.FirstOrDefault(u => u.FileName == uploadMessage.FileName && u.ClientId == uploadMessage.ClientId) ?? throw new InvalidOperationException("Upload record not found");

    uploadRecord.Submitted = records.Count;
    uploadRecord.Processed = 0;
    uploadRecord.Updated = 0;
    uploadRecord.Added = 0;
    uploadRecord.Invalid = 0;

    context.PersonalPropertySecurityUploads.Update(uploadRecord);
    await context.SaveChangesAsync();

    foreach (var record in records)
    {
      var personalPropertySecurity = MapRecord(record);
      personalPropertySecurity.ClientId = uploadMessage.ClientId;
      await ProcessRecord(personalPropertySecurity, uploadRecord, context);
    }
  }

  private async Task ProcessRecord(PersonalPropertySecurity newPPS, PersonalPropertySecurityUpload uploadRecord, PPSRegisterDbContext context)
  {
    var (isValid, validationResults) = ValidateRecord(newPPS);
    if (!isValid)
    {
      _logger.LogWarning("Invalid record: {Record} with validation errors: {ValidationResults}", newPPS, GetValidationErrors(validationResults));
      uploadRecord.Invalid++;
    }
    else
    {
      uploadRecord.Processed++;

      var existingRecord = context.PersonalPropertySecurities.FirstOrDefault(r =>
        r.GrantorFirstName == newPPS.GrantorFirstName &&
        r.GrantorLastName == newPPS.GrantorLastName &&
        r.VIN == newPPS.VIN &&
        r.SpgAcn == newPPS.SpgAcn &&
        r.ClientId == newPPS.ClientId
      );

      if (existingRecord != null)
      {
        //Skipping GrantorFirstName, GrantorLastName, VIN, SpgAcn
        existingRecord.GrantorMiddleNames = newPPS.GrantorMiddleNames;
        existingRecord.RegistrationDuration = newPPS.RegistrationDuration;
        existingRecord.RegistrationStartDate = newPPS.RegistrationStartDate;
        existingRecord.SpgOrganizationName = newPPS.SpgOrganizationName;
        uploadRecord.Updated++;
        context.PersonalPropertySecurities.Update(existingRecord);
      }
      else
      {
        context.PersonalPropertySecurities.Add(newPPS);
        uploadRecord.Added++;
      }
    }

    context.PersonalPropertySecurityUploads.Update(uploadRecord);
    await context.SaveChangesAsync();
  }

  private static PersonalPropertySecurity MapRecord(PersonalPropertySecurityRecord record)
  {
    return new PersonalPropertySecurity
    {
      GrantorFirstName = record.GrantorFirstName!,
      GrantorMiddleNames = record.GrantorMiddleNames,
      GrantorLastName = record.GrantorLastName!,
      VIN = record.VIN!,
      SpgAcn = record.SpgAcn!,
      SpgOrganizationName = record.SpgOrganizationName!,

      RegistrationStartDate = DateTime.TryParse(record.RegistrationStartDate, out var registrationStartDate) ? registrationStartDate : default,

      RegistrationDuration = record.RegistrationDuration!,
    };
  }

  private static (bool isValid, List<ValidationResult> validationResults) ValidateRecord(PersonalPropertySecurity record)
  {
    var validationContext = new ValidationContext(record);
    var validationResults = new List<ValidationResult>();

    var result = Validator.TryValidateObject(record, validationContext, validationResults, true);
    var validatorResults = (result, validationResults);

    var registrationDurationResult = IsValidRegistrationDuration(record);
    var startDateResult = IsValidStartDate(record);
    var spgAcnResult = IsValidSpgAcn(record);

    return CombineResults([validatorResults, registrationDurationResult, startDateResult, spgAcnResult]);
  }


  private static readonly string[] validRegistrationDurations = ["7", "25", "N/A"];
  private static (bool isValid, List<ValidationResult> validationResults) IsValidRegistrationDuration(PersonalPropertySecurity record)
  {
    return validRegistrationDurations.Contains(record.RegistrationDuration) ?
      (true, []) :
      (false, [new ValidationResult("Registration duration is invalid")]);
  }
  private static (bool isValid, List<ValidationResult> validationResults) IsValidStartDate(PersonalPropertySecurity record)
  {
    if (record.RegistrationStartDate <= DateTime.MinValue)
      return (false, [new ValidationResult("Registration start date is invalid")]);

    if (record.RegistrationStartDate.TimeOfDay != TimeSpan.Zero)
      return (false, [new ValidationResult("Registration start date must not include time information")]);

    return (true, []);
  }

  private static readonly string[] allowedSpaces = [" ", "-", ","];
  private static (bool isValid, List<ValidationResult> validationResults) IsValidSpgAcn(PersonalPropertySecurity record)
  {
    var trimmedAcn = new string([.. record.SpgAcn.Where(c => !allowedSpaces.Contains(c.ToString()))]);
    var digitCount = trimmedAcn.Count(char.IsDigit);

    if (digitCount != 9)
      return (false, [new ValidationResult("SPG ACN must contain exactly 9 digits")]);

    return (true, []);
  }

  private static (bool isValid, List<ValidationResult> validationResults) CombineResults(List<(bool isValid, List<ValidationResult> validationResults)> results)
  {
    return results.Aggregate((acc, curr) => (acc.isValid && curr.isValid, acc.validationResults.Concat(curr.validationResults).ToList()));
  }

  private static string GetValidationErrors(List<ValidationResult> validationResults) => string.Join(", ", validationResults.Select(r => r.ToString()));
}
