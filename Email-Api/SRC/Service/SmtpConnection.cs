using System.Diagnostics;
using Email_Api.Model;
using MailKit.Net.Smtp;
using MimeKit;

namespace Email_Api.Service;

public class SmtpConnection : ISmtpConnection
{
    private readonly IConfiguration _configuration;
    private readonly ISmtpClient _smtpClient;
    private readonly IEmailLogService _emailLogService;

    private readonly string host;
    private readonly int port;
    private readonly bool enableSsl;
    private readonly string username;
    private readonly string password;

    /// <summary>
    /// Uses either the environment variable settings if available, else falls back to application.json
    /// </summary>
    /// <param name="configuration">Added automatically through dependency injection</param>
    /// <param name="smtpClient">Injected SmtpClient instance for email sending</param>
    public SmtpConnection(
        IConfiguration configuration, 
        ISmtpClient smtpClient, 
        IEmailLogService emailLogService
        )
    {
        _configuration = configuration;
        _smtpClient = smtpClient;
        _emailLogService = emailLogService;

        host = Environment.GetEnvironmentVariable("EMAIL_API_HOST") ?? _configuration["Smtp:Host"] ?? "localhost";
        port = int.TryParse(Environment.GetEnvironmentVariable("EMAIL_API_PORT"), out int parsedPort)
            ? parsedPort
            : int.Parse(_configuration["Smtp:Port"] ?? "1025");

        enableSsl = bool.TryParse(Environment.GetEnvironmentVariable("EMAIL_API_ENABLE_SSL"), out bool parsedSsl)
            ? parsedSsl
            : bool.Parse(_configuration["Smtp:EnableSsl"] ?? "false");

        username = Environment.GetEnvironmentVariable("EMAIL_API_USER") ?? _configuration["Smtp:User"] ?? "";
        password = Environment.GetEnvironmentVariable("EMAIL_API_PASSWORD") ?? _configuration["Smtp:Password"] ?? "";

        Console.WriteLine($"SMTP Config: Host={host}, Port={port}, SSL={enableSsl}, User={username}");
    }

    public async Task SendEmailAsync(SingleEmailModel singleEmailModel)
    {
        await SendEmailAsync(singleEmailModel.To, singleEmailModel.Subject, singleEmailModel.Body, singleEmailModel.FromName, singleEmailModel.FromEmail);
    }

    public async Task SendEmailAsync(string toEmail, string subject, string body, string fromName = "HavenLabs", string fromEmail = "havenlabs@havenlabs.com")
    {
        _emailLogService.CreateEmailLog(toEmail, subject, body);

        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(fromName, fromEmail));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        email.Body = bodyBuilder.ToMessageBody();

        await _smtpClient.ConnectAsync(host, port, enableSsl);

        if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
        {
            await _smtpClient.AuthenticateAsync(username, password);
        }

        await _smtpClient.SendAsync(email);
        await _smtpClient.DisconnectAsync(true);
    }
}
