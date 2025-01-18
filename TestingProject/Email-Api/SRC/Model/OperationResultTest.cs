using Email_Api.Model;

namespace TestingProject.Email_Api.SRC.Model;

[TestFixture]
public class OperationResultTest
{
    [Test]
    public void Constructor_Should_InitializeProperties_With_DefaultValues()
    {
        // Act
        var operationResult = new OperationResult();

        // Assert
        Assert.IsNull(operationResult.Result);
        Assert.IsNull(operationResult.Message);
        Assert.IsFalse(operationResult.Success);
    }

    [Test]
    public void Constructor_Should_InitializeProperties_With_GivenValues()
    {
        // Arrange
        string expectedResult = "SuccessResult";
        string expectedMessage = "Operation completed successfully";
        bool expectedSuccess = true;

        // Act
        var operationResult = new OperationResult(expectedResult, expectedMessage, expectedSuccess);

        // Assert
        Assert.That(operationResult.Result, Is.EqualTo(expectedResult));
        Assert.That(operationResult.Message, Is.EqualTo(expectedMessage));
        Assert.IsTrue(operationResult.Success);
    }
}