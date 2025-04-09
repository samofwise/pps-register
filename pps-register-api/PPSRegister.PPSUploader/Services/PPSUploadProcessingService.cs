using Microsoft.EntityFrameworkCore;
using PPSRegister.Data;
using PPSRegister.Data.Models;

namespace PPSRegister.PPSUploader.Services;

public interface IPPSUploadProcessingService
{
  Task ProcessUpload(PPSUploadMessage? uploadMessage);
}

public class PPSUploadProcessingService(IDbContextFactory<PPSRegisterDbContext> _contextFactory) : IPPSUploadProcessingService
{
  public async Task ProcessUpload(PPSUploadMessage? uploadMessage)
  {
    if (uploadMessage == null) return;

    using var context = await _contextFactory.CreateDbContextAsync();

    // TODO: Implement the logic to process the upload
    // This might involve reading the CSV file, processing each record, and saving the data to the database
  }
}
