// ============================================================
// Module 4: Email Service — SMTP notification
// Owner: Shamraiz (02-131232-112)
//
// NuGet: dotnet add package MailKit
// ============================================================
using MailKit.Net.Smtp;
using MimeKit;

public class EmailService
{
    private readonly string _host;
    private readonly int    _port;
    private readonly string _user;
    private readonly string _password;
    private readonly string _from;

    public EmailService()
    {
        _host     = Environment.GetEnvironmentVariable("SMTP_HOST")     ?? "smtp.gmail.com";
        _port     = int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT") ?? "587");
        _user     = Environment.GetEnvironmentVariable("SMTP_USER")     ?? throw new Exception("SMTP_USER not set");
        _password = Environment.GetEnvironmentVariable("SMTP_PASSWORD") ?? throw new Exception("SMTP_PASSWORD not set");
        _from     = Environment.GetEnvironmentVariable("SMTP_FROM")     ?? _user;
    }

    public async Task SendReportEmailAsync(string toEmail, string reportUrl, string taskId)
    {
        var message = new MimeMessage();
        message.From.Add(MailboxAddress.Parse(_from));
        message.To.Add(MailboxAddress.Parse(toEmail));
        message.Subject = $"[IntelliFlow] Your report is ready — Task {taskId[..8]}";

        message.Body = new TextPart("plain")
        {
            Text = $"""
                    Hello,

                    Your IntelliFlow research report has been generated successfully.

                    Task ID   : {taskId}
                    Report URL: {reportUrl}

                    The report is available for download at the link above.
                    A blockchain audit record has also been created for this output.

                    — IntelliFlow Automated System
                    Bahria University Karachi | Cloud Computing Lab
                    """
        };

        using var smtp = new SmtpClient();
        await smtp.ConnectAsync(_host, _port, MailKit.Security.SecureSocketOptions.StartTls);
        await smtp.AuthenticateAsync(_user, _password);
        await smtp.SendAsync(message);
        await smtp.DisconnectAsync(quit: true);
    }
}
