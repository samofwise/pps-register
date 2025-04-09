using Amazon.SQS;
using Microsoft.EntityFrameworkCore;
using PPSRegister.Data;
using PPSRegister.Data.Models;
using System.Text.Json;
using Amazon.SQS.Model;

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

    if (_context.PersonalPropertySecurityUploads.Any(u => u.FileName == file.FileName))
      throw new InvalidOperationException("File has already been uploaded");

    var upload = new PersonalPropertySecurityUpload
    {
      FileName = file.FileName,
      ClientId = clientId
    };

    _context.PersonalPropertySecurityUploads.Add(upload);
    await _context.SaveChangesAsync();

    var request = await CreateSendMessageRequest(file, clientId);
    await sqsClient.SendMessageAsync(request);

    return upload;
  }

  private async Task<SendMessageRequest> CreateSendMessageRequest(IFormFile file, int clientId)
  {
    using var reader = new StreamReader(file.OpenReadStream());
    var data = await reader.ReadToEndAsync();

    var message = new PPSUploadMessage
    {
      FileName = file.FileName,
      ClientId = clientId,
      Data = data
    };

    return new SendMessageRequest
    {
      QueueUrl = _queueUrl,
      MessageBody = JsonSerializer.Serialize(message)
    };
  }
}

