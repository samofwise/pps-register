using Microsoft.AspNetCore.Mvc;

namespace PPSRegister.PPSUploader.Controllers;

[ApiController]
[Route("[controller]")]
public class UploaderController : ControllerBase
{
  private readonly PPSUploadWorker _uploader;
  private readonly ILogger<UploaderController> _logger;

  public UploaderController(
      PPSUploadWorker uploader,
      ILogger<UploaderController> logger)
  {
    _uploader = uploader;
    _logger = logger;
  }

  [HttpPost("upload")]
  public async Task<IActionResult> UploadFile(IFormFile file)
  {
    if (file == null || file.Length == 0)
    {
      return BadRequest("No file uploaded");
    }

    if (!file.ContentType.Equals("text/csv", StringComparison.OrdinalIgnoreCase))
    {
      return BadRequest("Only CSV files are allowed");
    }

    await _uploader.EnqueueFileAsync(file);

    _logger.LogInformation("File enqueued for processing: {FileName}", file.FileName);

    return Accepted(new { message = "File accepted for processing" });
  }
}