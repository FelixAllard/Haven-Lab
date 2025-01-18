using Email_Api.Exceptions;

namespace TestingProject.Email_Api.SRC.Exceptions;

[TestFixture]
public class TriedToFillEmailFieldWithEmptyWhiteSpaceTests
{
    [Test]
    public void Constructor_WithoutMessage_ShouldSetDefaultMessage()
    {
        // Arrange & Act
        var exception = new TriedToFillEmailFieldWithEmptyWhiteSpace();

        // Assert
        Assert.That(exception.Message, Does.Contain("Exception of type"));
    }

    [Test]
    public void Constructor_WithMessage_ShouldIncludeMessage()
    {
        // Arrange
        string customMessage = "Field cannot be empty or just whitespace";

        // Act
        var exception = new TriedToFillEmailFieldWithEmptyWhiteSpace(customMessage);

        // Assert
        Assert.That(exception.Message, Is.EqualTo("Tried to fill email field with an empty whitespace : " + customMessage));
    }

    [Test]
    public void Constructor_WithMessageAndInnerException_ShouldSetMessageAndInnerException()
    {
        // Arrange
        string customMessage = "Field cannot be empty or just whitespace";
        var innerException = new InvalidOperationException("Inner exception message");

        // Act
        var exception = new TriedToFillEmailFieldWithEmptyWhiteSpace(customMessage, innerException);

        // Assert
        Assert.That(exception.Message, Is.EqualTo(customMessage));
        Assert.That(exception.InnerException, Is.EqualTo(innerException));
    }
}