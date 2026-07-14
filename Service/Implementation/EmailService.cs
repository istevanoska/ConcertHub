using Domain.Config;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using Service.Interface;

namespace Service.Implementation;

public class EmailService : IEmailService
{
    private readonly EmailSettings _settings;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailSettings> settings, ILogger<EmailService> logger)
    {
        _settings = settings.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toAddress, string subject, string htmlBody)
    {
        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_settings.FromName, _settings.FromAddress));
        message.To.Add(MailboxAddress.Parse(toAddress));
        message.Subject = subject;
        message.Body = new BodyBuilder { HtmlBody = htmlBody }.ToMessageBody();

        if (!_settings.Enabled)
        {
            await WriteToOutboxAsync(toAddress, subject, htmlBody);
            return;
        }

        using var client = new SmtpClient();
        await client.ConnectAsync(_settings.Host, _settings.Port, SecureSocketOptions.StartTlsWhenAvailable);

        if (!string.IsNullOrWhiteSpace(_settings.Username))
            await client.AuthenticateAsync(_settings.Username, _settings.Password);

        await client.SendAsync(message);
        await client.DisconnectAsync(true);

        _logger.LogInformation("Email sent to {To} ({Subject})", toAddress, subject);
    }

    private async Task WriteToOutboxAsync(string toAddress, string subject, string htmlBody)
    {
        var outbox = Path.Combine("wwwroot", "outbox");
        Directory.CreateDirectory(outbox);
        var file = Path.Combine(outbox, $"{DateTime.UtcNow:yyyyMMddHHmmssfff}_{Guid.NewGuid():N}.html");
        await File.WriteAllTextAsync(file,
            $"<!-- To: {toAddress} | Subject: {subject} -->\n{htmlBody}");
        _logger.LogInformation("Email (SMTP disabled) written to outbox: {File}", file);
    }
}
