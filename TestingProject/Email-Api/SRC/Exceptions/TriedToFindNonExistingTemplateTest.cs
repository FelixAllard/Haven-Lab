using Email_Api.Exceptions;

namespace TestingProject.Email_Api.SRC.Exceptions;

[TestFixture]
public class TriedToFindNonExistingTemplateTests
{
    [Test]
    public void DefaultConstructor_Should_CreateException_WithNoMessage()
    {
        // Act
        var exception = new TriedToFindNonExistingTemplate();

        // Assert
        Assert.IsNotNull(exception);
    }

    [Test]
    public void Constructor_WithMessage_Should_SetMessageCorrectly()
    {
        // Arrange
        string expectedMessage = "Template not found";

        // Act
        var exception = new TriedToFindNonExistingTemplate(expectedMessage);

        // Assert
        Assert.That(exception.Message, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_Should_SetInnerExceptionCorrectly()
    {
        // Arrange
        string errorMessage = "An error occurred";
        var innerException = new Exception("Inner exception message");

        // Act
        var exception = new TriedToFindNonExistingTemplate(errorMessage, innerException);

        // Assert
        Assert.That(exception.Message, Is.EqualTo(errorMessage));
        Assert.That(exception.InnerException, Is.EqualTo(innerException));
    }
}