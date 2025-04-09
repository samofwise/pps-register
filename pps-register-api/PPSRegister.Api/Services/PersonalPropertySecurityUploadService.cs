using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using PPSRegister.Data;
using PPSRegister.Data.Models;
using System.Text.Json;
using Amazon.SQS.Model;
using System.Text.RegularExpressions;

namespace PPSRegister.Api.Services;

public interface IPersonalPropertySecurityUploadService
{
  Task<List<PersonalPropertySecurityUpload>> GetUploads(int clientId);
  Task<PersonalPropertySecurityUpload> UploadFile(IFormFile file, int clientId);
}

public class PersonalPropertySecurityUploadService(
  PPSRegisterDbContext _context,
  IAmazonSQS sqsClient,
  IConfiguration configuration) : IPersonalPropertySecurityUploadService
{
  private readonly string _queueUrl = configuration.GetValue<string>("AWS:SQS:QueueUrl")
    ?? throw new ArgumentNullException("AWS:SQS:QueueUrl not configured");

  public async Task<List<PersonalPropertySecurityUpload>> GetUploads(int clientId)
  {
    return await _context.PersonalPropertySecurityUploads
        .Where(u => u.ClientId == clientId)
        .ToListAsync();
  }

  public async Task<PersonalPropertySecurityUpload> UploadFile(IFormFile file, int clientId)
  {
    if (file == null)
      throw new InvalidOperationException("File is required");

    if (file.Length == 0)
      throw new InvalidOperationException("File is empty");

    if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
      throw new InvalidOperationException("File must be a CSV file");

    if (file.Length > 25 * 1024 * 1024)
      throw new InvalidOperationException("File is too large");

    if (_context.PersonalPropertySecurityUploads.Any(u => u.FileName == file.FileName))
      throw new InvalidOperationException("File has already been uploaded");

    var uploadMessage = await GetPPSUploadMessage(file, clientId);

    if (!ValidatePPSUploadMessage(uploadMessage))
      throw new InvalidOperationException("File is not a valid Personal Property Security upload");

    var upload = new PersonalPropertySecurityUpload
    {
      FileName = file.FileName,
      ClientId = clientId
    };

    _context.PersonalPropertySecurityUploads.Add(upload);
    await _context.SaveChangesAsync();

    var message = CreateSendMessageRequest(uploadMessage);
    await sqsClient.SendMessageAsync(message);

    return upload;
  }

  private static async Task<PPSUploadMessage> GetPPSUploadMessage(IFormFile file, int clientId)
  {
    using var reader = new StreamReader(file.OpenReadStream());
    var data = await reader.ReadToEndAsync();

    return new PPSUploadMessage
    {
      FileName = file.FileName,
      ClientId = clientId,
      Data = data
    };
  }

  private SendMessageRequest CreateSendMessageRequest(PPSUploadMessage message)
  {

    return new SendMessageRequest
    {
      QueueUrl = _queueUrl,
      MessageBody = JsonSerializer.Serialize(message)
    };
  }

  private const string headerAndRowPattern = @"^Grantor First Name,Grantor Middle Names,Grantor Last Name,VIN,Registration start date,Registration duration,SPG ACN,SPG Organization Name\r?\n(?:[^,\r\n]*,){7}[^,\r\n]*";

  private static bool ValidatePPSUploadMessage(PPSUploadMessage message)
  {
    Console.WriteLine(message.Data);
    Console.WriteLine(headerAndRowPattern);
    return Regex.IsMatch(
        message.Data,
        headerAndRowPattern,
        RegexOptions.Multiline
    );
  }
}

