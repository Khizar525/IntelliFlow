using MailKit.Net.Smtp;
using MimeKit;

namespace Notifier.API.Services;

public class EmailService
{
    private readonly string _smtpUser;
    private readonly string _smtpPassword;

    public EmailService()
    {
        _smtpUser = Environment.GetEnvironmentVariable("SMTP_USER")!;
        _smtpPassword = Environment.GetEnvironmentVariable("SMTP_PASSWORD")!;
    }

    public async Task SendReportEmailAsync(string toEmail, string taskId, string reportUrl)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_smtpUser));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = $"IntelliFlow Report Ready — Task {taskId}";

        message.Body = new TextPart("html")
        {
            Text = $@"
                <h2>Your IntelliFlow Report is Ready</h2>
                <p>Task ID: <strong>{taskId}</strong></p>
                <p><a href='{reportUrl}'>Click here to download your report</a></p>
                <br/>
                <p>— IntelliFlow System</p>"
        };

        using var client = new SmtpClient();
        await client.ConnectAsync("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_smtpUser, _smtpPassword);
        await client.SendAsync(message);
        await client.DisconnectAsync(true);
    }
}