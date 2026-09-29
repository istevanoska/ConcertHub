namespace Service.Interface;

public interface IQrCodeService
{
    byte[] GeneratePng(string content, int pixelsPerModule = 10);
}
