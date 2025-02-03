using Email_Api.Database;
using Email_Api.Service;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace TestingProject.Email_Api.SRC.Service;

[TestFixture]
public class EmailLogServiceTest
{
    private ApplicationDbContext _dbContext;
    private EmailLogService _emailLogService;

    [SetUp]
    public void SetUp()
    {
        // Use InMemory database for testing
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestEmailDatabase")
            .Options;

        _dbContext = new ApplicationDbContext(options);
        _emailLogService = new EmailLogService(_dbContext);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext.Database.EnsureDeleted();
        _dbContext.Dispose();
    }

    [Test]
    public void CreateEmailLog_ReturnsSentEmail_WithCorrectData()
    {
        // Arrange
        string email = "test@example.com";
        string subject = "Test Subject";
        string body = "Test Body";

        // Act
        var result = _emailLogService.CreateEmailLog(email, subject, body);

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.RecipientEmail, Is.EqualTo(email));
        Assert.That(result.EmailSubject, Is.EqualTo(subject));
        Assert.That(result.EmailBody, Is.EqualTo(body));
    }

    [Test]
    public async Task GetSentEmailsAsync_ReturnsListOfSentEmails()
    {
        // Arrange
        _dbContext.SentEmails.Add(new SentEmail { RecipientEmail = "test1@example.com", EmailSubject = "Subject1", EmailBody = "Body1" });
        _dbContext.SentEmails.Add(new SentEmail { RecipientEmail = "test2@example.com", EmailSubject = "Subject2", EmailBody = "Body2" });
        _dbContext.SaveChanges();

        // Act
        var result = await _emailLogService.GetSentEmailsAsync();

        // Assert
        Assert.IsNotNull(result);
        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result[0].RecipientEmail, Is.EqualTo("test1@example.com"));
        Assert.That(result[1].RecipientEmail, Is.EqualTo("test2@example.com"));
    }

    [Test]
    public void CreateEmailLog_SavesEmailToDatabase()
    {
        // Arrange
        string email = "save@example.com";
        string subject = "Save Subject";
        string body = "Save Body";

        // Act
        _emailLogService.CreateEmailLog(email, subject, body);

        // Assert
        var savedEmail = _dbContext.SentEmails.Find(1); // Assuming first entry has ID 1
        Assert.IsNotNull(savedEmail);
        Assert.That(savedEmail.RecipientEmail, Is.EqualTo(email));
        Assert.That(savedEmail.EmailSubject, Is.EqualTo(subject));
        Assert.That(savedEmail.EmailBody, Is.EqualTo(body));
    }
    
}