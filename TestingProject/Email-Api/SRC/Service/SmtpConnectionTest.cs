using Email_Api.Model;
using Email_Api.Service;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using MimeKit;
using Moq;

namespace TestingProject.Email_Api.SRC.Service;
[TestFixture]
public class SmtpConnectionTest
{
    private Mock<IConfiguration> _mockConfiguration;
    private Mock<ISmtpClient> _mockSmtpClient;
    private SmtpConnection _smtpConnection;


    [SetUp]
    public void Setup()
    {
        _mockConfiguration = new Mock<IConfiguration>();
        _mockSmtpClient = new Mock<ISmtpClient>();

        _mockConfiguration.Setup(config => config["Smtp:Host"]).Returns("smtp.testserver.com");
        _mockConfiguration.Setup(config => config["Smtp:Port"]).Returns("1025");

        _smtpConnection = new SmtpConnection(
            _mockConfiguration.Object, 
            _mockSmtpClient.Object);
    }

    [Test]
    public async Task SendEmailAsync_SingleEmailModel_Success()
    {
        // Arrange
        var singleEmailModel = new SingleEmailModel
        {
            FromName = "Test Sender",
            FromEmail = "sender@test.com",
            To = "recipient@test.com",
            Subject = "Test Subject",
            Body = "<h1>Test Body</h1>"
        };

        _mockSmtpClient.Setup(client => client.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), false,default))
            .Returns(Task.CompletedTask);
        _mockSmtpClient.Setup(client => client.SendAsync(It.IsAny<MimeMessage>(), default, default))
            .Returns(Task.FromResult("Mocked result string"));
        _mockSmtpClient.Setup(client => client.DisconnectAsync(true,default))
            .Returns(Task.CompletedTask);

        // Act
        await _smtpConnection.SendEmailAsync(singleEmailModel);

        // Assert
        _mockSmtpClient.Verify(client => client.ConnectAsync("smtp.testserver.com", 1025, false,default), Times.Once);
        _mockSmtpClient.Verify(client => client.SendAsync(It.IsAny<MimeMessage>(),default,default), Times.Once);
        _mockSmtpClient.Verify(client => client.DisconnectAsync(true,default), Times.Once);
    }
    
    [Test]
    public async Task SendEmailAsync_WithParameters_Success()
    {
        // Arrange
        var toEmail = "recipient@test.com";
        var subject = "Test Subject";
        var body = "<h1>Test Body</h1>";
        var fromName = "Custom Sender";
        var fromEmail = "customsender@test.com";
    
        _mockSmtpClient.Setup(client => client.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), false,default))
            .Returns(Task.CompletedTask);
        _mockSmtpClient.Setup(client => client.SendAsync(It.IsAny<MimeMessage>(),default, default))
            .Returns(Task.FromResult("Mocked result string"));
        _mockSmtpClient.Setup(client => client.DisconnectAsync(true,default))
            .Returns(Task.CompletedTask);
    
        // Act
        await _smtpConnection.SendEmailAsync(toEmail, subject, body, fromName, fromEmail);
    
        // Assert
        _mockSmtpClient.Verify(client => client.ConnectAsync("smtp.testserver.com", 1025, false,default), Times.Once);
        _mockSmtpClient.Verify(client => client.SendAsync(It.IsAny<MimeMessage>(),default,default), Times.Once);
        _mockSmtpClient.Verify(client => client.DisconnectAsync(true,default), Times.Once);
    }
    
    [Test]
    public async Task SendEmailAsync_UsesDefaultFromNameAndEmail_Success()
    {
        // Arrange
        var toEmail = "recipient@test.com";
        var subject = "Test Subject";
        var body = "<h1>Test Body</h1>";
    
        _mockSmtpClient.Setup(client => client.ConnectAsync(It.IsAny<string>(), It.IsAny<int>(), false,default))
            .Returns(Task.CompletedTask);
        _mockSmtpClient.Setup(client => client.SendAsync(It.IsAny<MimeMessage>(),default, default))
            .Returns(Task.FromResult("Mocked result string"));
        _mockSmtpClient.Setup(client => client.DisconnectAsync(true,default))
            .Returns(Task.CompletedTask);
    
        // Act
        await _smtpConnection.SendEmailAsync(toEmail, subject, body);
    
        // Assert
        _mockSmtpClient.Verify(client => client.ConnectAsync("smtp.testserver.com", 1025, false,default), Times.Once);
        _mockSmtpClient.Verify(client => client.SendAsync(It.IsAny<MimeMessage>(),default,default), Times.Once);
        _mockSmtpClient.Verify(client => client.DisconnectAsync(true,default), Times.Once);
    }
}