using Email_Api.Database;

namespace TestingProject.Email_Api.SRC.Database;

[TestFixture]
public class SentEmailTest
{
    [Test]
    public void SentEmail_Properties_AreSetCorrectly()
    {
        // Arrange
        var email = new SentEmail
        {
            Id = 1,
            RecipientEmail = "test@example.com",
            EmailSubject = "Test Subject",
            EmailBody = "Test Body"
        };

        // Assert
        Assert.That(email.Id, Is.EqualTo(1));
        Assert.That(email.RecipientEmail, Is.EqualTo("test@example.com"));
        Assert.That(email.EmailSubject, Is.EqualTo("Test Subject"));
        Assert.That(email.EmailBody, Is.EqualTo("Test Body"));
    }

    [Test]
    public void SentEmail_DefaultConstructor_Properties_AreNull()
    {
        // Arrange
        var email = new SentEmail();

        // Assert
        Assert.That(email.Id, Is.EqualTo(0));  // Default value for int is 0
        Assert.IsNull(email.RecipientEmail);
        Assert.IsNull(email.EmailSubject);
        Assert.IsNull(email.EmailBody);
    }
}