using Email_Api.Model;

namespace TestingProject.Email_Api.SRC.Model;
[TestFixture]
public class DirectEmailModelTest
{
    [Test]
    public void Constructor_Default_ShouldCreateInstance()
    {
        // Arrange & Act
        var model = new DirectEmailModel();

        // Assert
        Assert.NotNull(model);
    }

    [Test]
    public void Constructor_WithParameters_ShouldSetProperties()
    {
        // Arrange
        string emailToSendTo = "test@example.com";
        string emailTitle = "Test Email";
        string templateName = "TestTemplate";
        string header = "Header Content";
        string body = "Body Content";
        string footer = "Footer Content";
        string correspondantName = "John Doe";
        string senderName = "Admin";

        // Act
        var model = new DirectEmailModel(
            emailToSendTo, emailTitle, templateName, header, body, footer, correspondantName, senderName);

        // Assert
        Assert.That(model.EmailToSendTo, Is.EqualTo(emailToSendTo));
        Assert.That(model.EmailTitle, Is.EqualTo(emailTitle));
        Assert.That(model.TemplateName, Is.EqualTo(templateName));
        Assert.That(model.Header, Is.EqualTo(header));
        Assert.That(model.Body, Is.EqualTo(body));
        Assert.That(model.Footer, Is.EqualTo(footer));
        Assert.That(model.CorrespondantName, Is.EqualTo(correspondantName));
        Assert.That(model.SenderName, Is.EqualTo(senderName));
    }

    [Test]
    public void Properties_ShouldBeSettable()
    {
        // Arrange
        var model = new DirectEmailModel();

        // Act
        model.EmailToSendTo = "test@example.com";
        model.EmailTitle = "Test Email";
        model.TemplateName = "TestTemplate";
        model.Header = "Header Content";
        model.Body = "Body Content";
        model.Footer = "Footer Content";
        model.CorrespondantName = "John Doe";
        model.SenderName = "Admin";

        // Assert
        Assert.That(model.EmailToSendTo, Is.EqualTo("test@example.com"));
        Assert.That(model.EmailTitle, Is.EqualTo("Test Email"));
        Assert.That(model.TemplateName, Is.EqualTo("TestTemplate"));
        Assert.That(model.Header, Is.EqualTo("Header Content"));
        Assert.That(model.Body, Is.EqualTo("Body Content"));
        Assert.That(model.Footer, Is.EqualTo("Footer Content"));
        Assert.That(model.CorrespondantName, Is.EqualTo("John Doe"));
        Assert.That(model.SenderName, Is.EqualTo("Admin"));
    }

    [Test]
    public void IsEmpty_ShouldReturnTrue_WhenAllPropertiesAreNull()
    {
        // Arrange
        var model = new DirectEmailModel();

        // Act
        bool result = model.IsEmpty();

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void IsEmpty_ShouldReturnFalse_WhenAtLeastOnePropertyIsNotNull()
    {
        // Arrange
        var model = new DirectEmailModel();
        model.EmailToSendTo = "test@example.com"; // Setting one property

        // Act
        bool result = model.IsEmpty();

        // Assert
        Assert.IsFalse(result);
    }
}