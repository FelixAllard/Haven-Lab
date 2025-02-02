using Email_Api.Database;
using Microsoft.EntityFrameworkCore;

namespace Email_Api.Service;

public class EmailLogService : IEmailLogService
{
    public ApplicationDbContext _context { get; set; }
    public EmailLogService(ApplicationDbContext dbContext)
    {
        _context = dbContext;
        
    }
    public SentEmail CreateEmailLog(string email, string subject, string body)
    {
        var sentEmail = new SentEmail
        {
            EmailBody = body,
            EmailSubject = subject,
            RecipientEmail = email
        };

        Console.WriteLine("We sent the email");
        _context.SentEmails.Add(sentEmail);
        _context.SaveChanges();

        return sentEmail;
    }


    public async Task<List<SentEmail>> GetSentEmailsAsync()
    {
        return await _context.SentEmails.ToListAsync();
    }

}