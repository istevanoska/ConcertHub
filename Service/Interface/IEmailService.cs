namespace Service.Interface;

public interface IEmailService
{
    Task SendAsync(string toAddress, string subject, string htmlBody);
}
