using Email_Api.Model;

namespace TestingProject.Email_Api.SRC.Model;

[TestFixture]
public class SingleEmailModelTest
{
    [Test]
    public void Constructor_Should_InitializeProperties_With_DefaultValues()
    {
        // Act
        var emailModel = new SingleEmailModel();

        // Assert
        Assert.IsNull(emailModel.FromName);
        Assert.IsNull(emailModel.FromEmail);
        Assert.IsNull(emailModel.To);
        Assert.IsNull(emailModel.Subject);
        Assert.IsNull(emailModel.Body);
    }

    [Test]
    public void Properties_Should_SetAndGet_CorrectValues()
    {
        // Arrange
        var emailModel = new SingleEmailModel();
        string expectedFromName = "John Doe";
        string expectedFromEmail = "john.doe@example.com";
        string expectedTo = "jane.doe@example.com";
        string expectedSubject = "Test Subject";
        string expectedBody = "This is a test email.";

        // Act
        emailModel.FromName = expectedFromName;
        emailModel.FromEmail = expectedFromEmail;
        emailModel.To = expectedTo;
        emailModel.Subject = expectedSubject;
        emailModel.Body = expectedBody;

        // Assert
        Assert.That(emailModel.FromName, Is.EqualTo(expectedFromName));
        Assert.That(emailModel.FromEmail, Is.EqualTo(expectedFromEmail));
        Assert.That(emailModel.To, Is.EqualTo(expectedTo));
        Assert.That(emailModel.Subject, Is.EqualTo(expectedSubject));
        Assert.That(emailModel.Body, Is.EqualTo(expectedBody));
    }
}
