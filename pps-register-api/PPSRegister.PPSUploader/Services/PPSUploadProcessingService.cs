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

    var records = csv.GetRecords<PersonalPropertySecurity>().ToList();

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
      record.ClientId = uploadMessage.ClientId;
      await ProcessRecord(record, uploadRecord, context);
    }
  }

  private async Task ProcessRecord(PersonalPropertySecurity record, PersonalPropertySecurityUpload uploadRecord, PPSRegisterDbContext context)
  {
    var (isValid, validationResults) = ValidateRecord(record);
    if (!isValid)
    {
      _logger.LogWarning("Invalid record: {Record} with validation errors: {ValidationResults}", record, GetValidationErrors(validationResults));
      uploadRecord.Invalid++;
    }
    else
    {
      uploadRecord.Processed++;

      var existingRecord = context.PersonalPropertySecurities.FirstOrDefault(r =>
        r.GrantorFirstName == record.GrantorFirstName &&
        r.GrantorLastName == record.GrantorLastName &&
        r.VIN == record.VIN &&
        r.SpgAcn == record.SpgAcn &&
        r.SpgOrganizationName == record.SpgOrganizationName &&
        r.ClientId == record.ClientId
      );

      if (existingRecord != null)
      {
        //Skipping GrantorFirstName, GrantorLastName, VIN, SpgAcn, SpgOrganizationName
        existingRecord.GrantorMiddleNames = record.GrantorMiddleNames;
        existingRecord.RegistrationDuration = record.RegistrationDuration;
        existingRecord.RegistrationStartDate = record.RegistrationStartDate;
        existingRecord.SpgOrganizationName = record.SpgOrganizationName;
        uploadRecord.Updated++;
        context.PersonalPropertySecurities.Update(existingRecord);
      }
      else
      {
        context.PersonalPropertySecurities.Add(record);
        uploadRecord.Added++;
      }
    }

    context.PersonalPropertySecurityUploads.Update(uploadRecord);
    await context.SaveChangesAsync();
  }

  private static (bool isValid, List<ValidationResult> validationResults) ValidateRecord(PersonalPropertySecurity record)
  {
    var validationContext = new ValidationContext(record);
    var validationResults = new List<ValidationResult>();

    var result = Validator.TryValidateObject(record, validationContext, validationResults, true);

    return (result, validationResults);
  }

  private static string GetValidationErrors(List<ValidationResult> validationResults) => string.Join(", ", validationResults.Select(r => r.ToString()));
}
