using Email_Api.Exceptions;

namespace TestingProject.Email_Api.SRC.Exceptions;

[TestFixture]
public class BadEmailModelTest
{
    [Test]
    public void DefaultConstructor_Should_CreateException_WithNoMessage()
    {
        // Act
        var exception = new BadEmailModel();

        // Assert
        Assert.IsNotNull(exception);

    }

    [Test]
    public void Constructor_WithMessage_Should_SetMessageCorrectly()
    {
        // Arrange
        string fieldName = "Subject";
        string expectedMessage = "Could not build message because field " + fieldName;

        // Act
        var exception = new BadEmailModel(fieldName);

        // Assert
        Assert.That(exception.Message, Is.EqualTo(expectedMessage));
    }

    [Test]
    public void Constructor_WithMessageAndPlaceholder_Should_SetMessageCorrectly()
    {
        // Arrange
        string fieldName = "Body";
        string expectedMessage = "Could not build message because field " + fieldName;

        // Act
        var exception = new BadEmailModel(fieldName, "{placeholder}");

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
        var exception = new BadEmailModel(errorMessage, innerException);

        // Assert
        Assert.That(exception.Message, Is.EqualTo(errorMessage));
        Assert.That(exception.InnerException, Is.EqualTo(innerException));
    }
}