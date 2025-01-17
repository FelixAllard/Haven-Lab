using System.Diagnostics;
using Email_Api.Model;
using MailKit.Net.Smtp;
using MimeKit;

namespace Email_Api.Service;

public class SmtpConnection : ISmtpConnection
{
    private readonly IConfiguration _configuration;
    public readonly string hostMailDev;

    /// <summary>
    /// Uses either the docker address if in a docker environment, else use the application.json address
    /// </summary>
    ///<remarks>
    /// The docker address does not have http:// before it, but the host address does</remarks>
    /// <param name="configuration">Added automatically through dependency injection</param>
    public SmtpConnection(IConfiguration configuration)
    {
        _configuration = configuration;
        hostMailDev = Environment.GetEnvironmentVariable("BASE_URL_MAIL_DEV_SERVER") ?? _configuration["Smtp:Host"]??"localhost";
        Console.WriteLine($"Host: {hostMailDev}" + $"Port : {_configuration["Smtp:Port"]}");
    }

    public async Task SendEmailAsync(SingleEmailModel singleEmailModel)
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(singleEmailModel.FromName, singleEmailModel.FromEmail));
        email.To.Add(new MailboxAddress("", singleEmailModel.To));
        email.Subject = singleEmailModel.Subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = singleEmailModel.Body };
        email.Body = bodyBuilder.ToMessageBody();

        using var _smtpClient = new SmtpClient();
        await _smtpClient.ConnectAsync(
            hostMailDev,
            int.Parse(_configuration["Smtp:Port"]??"1025"),
            false
        );
        await _smtpClient.SendAsync(email);
        await _smtpClient.DisconnectAsync(true);
    }
    
    public async Task SendEmailAsync(string toEmail, string subject, string body,string fromName = "HavenLabs", string fromEmail = "havenlabs@havenlabs.com")
    {
        var email = new MimeMessage();
        email.From.Add(new MailboxAddress(fromName, fromEmail));
        email.To.Add(new MailboxAddress("", toEmail));
        email.Subject = subject;

        var bodyBuilder = new BodyBuilder { HtmlBody = body };
        email.Body = bodyBuilder.ToMessageBody();

        using var _smtpClient = new SmtpClient();
        await _smtpClient.ConnectAsync(
            hostMailDev,
            int.Parse(_configuration["Smtp:Port"]??"1025"),
            false
        );
        await _smtpClient.SendAsync(email);
        await _smtpClient.DisconnectAsync(true);
    }
    
}