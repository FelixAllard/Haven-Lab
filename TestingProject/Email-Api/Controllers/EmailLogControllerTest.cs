using Email_Api.Controllers;
using Email_Api.Database;
using Email_Api.Model;
using Email_Api.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Email_Api.Controllers;

[TestFixture]
public class EmailLogControllerTest
{
    private Mock<IEmailLogService> _emailLogServiceMock;
    private EmailLogController _controller;

    [SetUp]
    public void SetUp()
    {
        _emailLogServiceMock = new Mock<IEmailLogService>();
        _controller = new EmailLogController(_emailLogServiceMock.Object);
    }

    [Test]
    public async Task GetEmailLogs_ReturnsOk_WithEmailLogs()
    {
        // Arrange
        var fakeLogs = new List<SentEmail>()
        {
            new SentEmail()
            {
                EmailBody = "test@test.com",
                EmailSubject = "test",
                RecipientEmail = "test@test.com",
            },
            new SentEmail()
            {
                EmailBody = "test@test.com",
                EmailSubject = "test",
                RecipientEmail = "test@test.com",
            }

        }; // Replace with actual EmailLog model if needed
        _emailLogServiceMock.Setup(s => s.GetSentEmailsAsync()).ReturnsAsync(fakeLogs);

        // Act
        var result = await _controller.GetEmailLogs();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.NotNull(okResult);
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
        Assert.That(okResult.Value, Is.EqualTo(fakeLogs));
    }

    [Test]
    public void GetEmailLogs_ThrowsException_ReturnsError()
    {
        // Arrange
        _emailLogServiceMock.Setup(s => s.GetSentEmailsAsync()).ThrowsAsync(new Exception("Test exception"));

        // Act & Assert
        var ex = Assert.ThrowsAsync<Exception>(async () => await _controller.GetEmailLogs());
        Assert.That(ex.Message, Is.EqualTo("Test exception"));
    }
    
}