using Email_Api.Exceptions;

namespace TestingProject.Email_Api.SRC.Exceptions;

[TestFixture]
public class EmailStringContainsPlaceholderTest
{
    [Test]
    public void DefaultConstructor_Should_CreateException_WithNoMessage()
    {
        // Act
        var exception = new EmailStringContainsPlaceholder();

        // Assert
        Assert.IsNotNull(exception);

    }

    [Test]
    public void Constructor_WithMessageAndPlaceholder_Should_SetMessageCorrectly()
    {
        // Arrange
        string fieldName = "Body";
        string placeholder = "{empty}";
        string expectedMessage = "Could not build message because field " + fieldName + " contains an empty placeholder : " + placeholder;

        // Act
        var exception = new EmailStringContainsPlaceholder(fieldName, placeholder);

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
        var exception = new EmailStringContainsPlaceholder(errorMessage, innerException);

        // Assert
        Assert.That(exception.Message, Is.EqualTo(errorMessage));
        Assert.That(exception.InnerException, Is.EqualTo(innerException));
    }
}
