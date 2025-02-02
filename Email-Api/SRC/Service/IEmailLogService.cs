using Email_Api.Database;

namespace Email_Api.Service;

public interface IEmailLogService
{
    public SentEmail CreateEmailLog(string email, string subject, string body);
    Task<List<SentEmail>> GetSentEmailsAsync();

}