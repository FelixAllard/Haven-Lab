using Email_Api.Model;

namespace Email_Api.Service;

public interface ISmtpConnection
{
    public Task SendEmailAsync(SingleEmailModel singleEmailModel);

    public Task SendEmailAsync(string toEmail, string subject, string body,
        string fromName = "haven.labs@havenlabs.com", string fromEmail = "Haven Labs");
}