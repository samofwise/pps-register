namespace PPSRegister.Data.Models;

public class PPSUploadMessage
{
  public string FileName { get; set; } = string.Empty;
  public int ClientId { get; set; }
  public string Data { get; set; } = string.Empty;
}