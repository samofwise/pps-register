namespace PPSRegister.PPSUploader;

public interface IPPSUploaderClient
{
  Task UploadFileAsync(FileUploadMessage message);
}