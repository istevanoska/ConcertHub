namespace Service.Interface;

public interface IFileUploadService
{
    Task<string> UploadFileAsync(
        byte[] fileBytes,
        string originalFileName,
        string folder = "refunds");
}
