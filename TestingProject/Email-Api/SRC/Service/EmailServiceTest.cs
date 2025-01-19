using Email_Api.Exceptions;
using Email_Api.Model;
using Email_Api.Service;
using Moq;

namespace TestingProject.Email_Api.SRC.Service;
[TestFixture]
public class EmailServiceTest
{
    private Mock<ISmtpConnection> _smtpConnection;
    private Mock<ITemplateManager> _templateManager;
    private EmailService _emailService;
    [SetUp]
    public void Setup()
    {
        _smtpConnection = new Mock<ISmtpConnection>();
        _templateManager = new Mock<ITemplateManager>();
        _emailService = new EmailService(
            _smtpConnection.Object, 
            _templateManager.Object
            );
    }
    [Test]
    public void SendEmail_ShouldReturnSuccess_WhenValidEmailIsProvided()
    {
        // Arrange
        var emailModel = new DirectEmailModel
        {
            EmailToSendTo = "test@example.com",
            EmailTitle = "Test Email",
            TemplateName = "Default",
            Header = "Test Header",
            Body = "Test Body",
            Footer = "Test Footer",
            CorrespondantName = "John Doe",
            SenderName = "Admin"
        };

        var mockTemplate = new EmailTemplate("%%EMAIL_BODY%%");

        _templateManager
            .Setup(x => x.GetTemplate(It.IsAny<string>()))
            .Returns(mockTemplate);

        _smtpConnection
            .Setup(x => x.SendEmailAsync(
                "Example", 
                "Example", 
                "Built Email Content", 
                default,
                default))
            .Returns(Task.CompletedTask);

        // Act
        var result = _emailService.SendEmail(emailModel);

        // Assert
        Assert.IsTrue(result.Success, "Expected success to be true.");
        Assert.That(result.Result, Is.EqualTo("Successfully sent email!"));
    }
    [TestCase("","Title")]
    [TestCase(null,"Title")]
    [TestCase("@gmail.com", "Title")]
    [TestCase("hello@world@gmail.com", "Title")]
    [TestCase("helloexample@gmail", "Title")]
    [TestCase("hello@gmail.com", "")]
    [TestCase("hello@gmail.com", null)]
    [Test]
    public void SendEmail_BadEmailModel_WhenInvalidInformation(string? emailToSendTo, string? emailTitle)
    {
        // Arrange
        var emailModel = new DirectEmailModel
        {
            EmailToSendTo = emailToSendTo,
            EmailTitle = emailTitle,
            TemplateName = "Default",
            Header = "Test Header",
            Body = "Test Body",
            Footer = "Test Footer",
            CorrespondantName = "John Doe",
            SenderName = "Admin"
        };
        // Act and Assert :)

        Assert.Throws<BadEmailModel>(() => _emailService.SendEmail(emailModel));
        
    }
    /// <summary>
    /// This might sound very stupid, but this test is only to check what happens if the template is null
    /// </summary>
    /// <param name="templateName"></param>
    [TestCase("")]
    [TestCase(null)]
    [Test]
    public void SendEmail_ShouldCallGetTemplateWithDefaultTemplate_WhenTemplateNameIsNullOrWhitespace(string? templateName)
    {
        // Arrange
        var emailModel = new DirectEmailModel
        {
            EmailToSendTo = "johndoe@example.com",
            EmailTitle = "A Title",
            TemplateName = templateName,
            Header = "Test Header",
            Body = "Test Body",
            Footer = "Test Footer",
            CorrespondantName = "John Doe",
            SenderName = "Admin"
        };

        var mockTemplate = new EmailTemplate("%%EMAIL_BODY%%");

        _templateManager
            .Setup(x => x.GetTemplate(It.IsAny<string>()))
            .Returns(mockTemplate);

        _smtpConnection
            .Setup(x => x.SendEmailAsync(
                "Example", 
                "Example", 
                "Built Email Content", 
                default,
                default))
            .Returns(Task.CompletedTask);
        // Act
        _emailService.SendEmail(emailModel);

        // Assert
        _templateManager.Verify(
            tm => tm.GetTemplate("Default"), 
            Times.Once, 
            "GetTemplate was not called with the 'Default' template."
        );
    }
    [Test]
    public void SendEmail_ShouldThrowKeyNotFoundException_WhenTemplateManagerThrowsIt()
    {
        // Arrange
        var emailModel = new DirectEmailModel
        {
            EmailToSendTo = "test@example.com",
            EmailTitle = "Test Email",
            TemplateName = "Default",
            Header = "Test Header",
            Body = "Test Body",
            Footer = "Test Footer",
            CorrespondantName = "John Doe",
            SenderName = "Admin"
        };

        var mockTemplate = new EmailTemplate("%%EMAIL_BODY%%");

        _templateManager
            .Setup(x => x.GetTemplate(It.IsAny<string>()))
            .Throws(new KeyNotFoundException());

        _smtpConnection
            .Setup(x => x.SendEmailAsync(
                "Example", 
                "Example", 
                "Built Email Content", 
                default,
                default))
            .Returns(Task.CompletedTask);

        // Act
        Assert.Throws<KeyNotFoundException>(() => _emailService.SendEmail(emailModel));
    }
    [Test]
    public void SendEmail_ShouldThrowTemplateRequiredFieldNotSet_WhenTemplateManagerThrowsIt()
    {
        // Arrange
        var emailModel = new DirectEmailModel
        {
            EmailToSendTo = "test@example.com",
            EmailTitle = "Test Email",
            TemplateName = "Default",
            Header = "Test Header",
            Body = "Test Body",
            Footer = "Test Footer",
            CorrespondantName = "John Doe",
            SenderName = "Admin"
        };

        var mockTemplate = new EmailTemplate("%%EMAIL_BODY%%");

        _templateManager
            .Setup(x => x.GetTemplate(It.IsAny<string>()))
            .Throws(new KeyNotFoundException());

        _smtpConnection
            .Setup(x => x.SendEmailAsync(
                "Example", 
                "Example", 
                "Built Email Content", 
                default,
                default))
            .Returns(Task.CompletedTask);

        // Act
        Assert.Throws<KeyNotFoundException>(() => _emailService.SendEmail(emailModel));
    }
    [Test]
    public void SendEmail_ShouldThrowTemplateRequiredFieldNotSet_WhenTemplateBuildThrowsIt()
    {
        // Arrange
        var emailModel = new DirectEmailModel
        {
            EmailToSendTo = "test@example.com",
            EmailTitle = "Test Email",
            TemplateName = "Default",
            Header = "Test Header",
            Body = "",
            Footer = "Test Footer",
            CorrespondantName = "John Doe",
            SenderName = "Admin"
        };

        var mockTemplate = new EmailTemplate("%%EMAIL_BODY%%");

        _templateManager
            .Setup(x => x.GetTemplate(It.IsAny<string>()))
            .Returns(mockTemplate);

        _smtpConnection
            .Setup(x => x.SendEmailAsync(
                "Example", 
                "Example", 
                "", 
                default,
                default))
            .Returns(Task.CompletedTask);

        // Act
        Assert.Throws<TemplateRequiredFieldNotSet>(() => _emailService.SendEmail(emailModel));
    }
    

    
}