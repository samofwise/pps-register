using PPSRegister.Data;
using PPSRegister.Data.Models;

namespace PPSRegister.PPSUploader.Services;

public interface IPPSUploadProcessingService
{
  Task ProcessUpload(PPSUploadMessage? uploadMessage);
}

public class PPSUploadProcessingService(PPSRegisterDbContext _context, ILogger<PPSUploadProcessingService> _logger) : IPPSUploadProcessingService
{
  public Task ProcessUpload(PPSUploadMessage? uploadMessage)
  {
    // TODO: Implement the logic to process the upload
    // This might involve reading the CSV file, processing each record, and saving the data to the database
    return Task.CompletedTask;
  }
}
