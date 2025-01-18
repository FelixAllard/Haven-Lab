using Castle.Components.DictionaryAdapter.Xml;
using Email_Api.Controllers;
using Email_Api.Exceptions;
using Email_Api.Model;
using Email_Api.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Newtonsoft.Json.Linq;
using Model_DirectEmailModel = Email_Api.Model.DirectEmailModel;

namespace TestingProject.Email_Api.Controllers;

[TestFixture]
public class EmailControllerTest
{
    private Mock<ISmtpConnection> mockSmtpConnection;
    private Mock<IEmailService> mockEmailService;
    private EmailController emailController;

    [SetUp]
    public void SetUp()
    {
        mockSmtpConnection = new Mock<ISmtpConnection>();
        mockEmailService = new Mock<IEmailService>();
        emailController = new EmailController(mockSmtpConnection.Object, mockEmailService.Object);
    }

    [Test]
    public async Task SendSingleEmail_ShouldReturnOk_WhenEmailSentSuccessfully()
    {
        DirectEmailModel directEmailModel = new Model_DirectEmailModel(
            "example@example.com",
            "Title",
            "Default",
            "header",
            "A random body",
            "footer",
            "A name",
            "The person who sent"
        );
        // Arrange
        mockEmailService.Setup(x => x.SendEmail(directEmailModel))
            .Returns(new OperationResult("RESULT", "MESSAGE", true));

        // Act
        var result = await emailController.SendSingleEmailWithTemplate(
            directEmailModel
        );

        // Assert
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult, "ObjectResult should not be null.");
        Assert.That(objectResult.StatusCode, Is.EqualTo(200));  // Ensure OK status

        // Ensure Value is not null
        Assert.IsNotNull(objectResult.Value, "ObjectResult.Value should not be null.");

        // Assert the Value is of type OperationResult
        var operationResult = objectResult.Value as OperationResult;
        Assert.IsNotNull(operationResult, "ObjectResult.Value should be an OperationResult.");

        // Check if the OperationResult's message is correct
        Assert.That(operationResult.Message, Is.EqualTo("MESSAGE"));
    }
    [Test]
    public async Task SendSingleEmail_ShouldReturnNoContent_WhenDirectEmailModelIsNull()
    {
        // Act
        var result = await emailController.SendSingleEmailWithTemplate(null);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult, "Expected NoContentResult, but got null.");
        Assert.That(noContentResult.StatusCode, Is.EqualTo(204)); // Check for 204 No Content
    }

    [Test]
    public async Task SendSingleEmail_ShouldReturnNoContent_WhenDirectEmailModelIsEmpty()
    {
        // Arrange
        var emptyEmailModel = new Model_DirectEmailModel(
            null, // Assuming these are the default values that make it empty
            null, 
            null,
            null,
            null,
            null,
            null,
            null
        );

        // Act
        var result = await emailController.SendSingleEmailWithTemplate(emptyEmailModel);

        // Assert
        var noContentResult = result as NoContentResult;
        Assert.IsNotNull(noContentResult, "Expected NoContentResult, but got null.");
        Assert.That(noContentResult.StatusCode, Is.EqualTo(204)); // Check for 204 No Content
    }
    [Test]
    public async Task SendSingleEmail_ShouldReturnBadRequest_WhenBadEmailModelExceptionIsThrown()
    {
        // Arrange: Ensure mock throws an exception
        mockEmailService.Setup(x => x.SendEmail(It.IsAny<Model_DirectEmailModel>()))
            .Throws(new BadEmailModel("Invalid email"));

        var emailModel = new Model_DirectEmailModel(
            "example@example.com",
            "Title",
            "Default",
            "header",
            "A random body",
            "footer",
            "A name",
            "The person who sent"
        );

        // Act
        var result = await emailController.SendSingleEmailWithTemplate(emailModel);

        // Assert: Ensure response is not null
        Assert.IsNotNull(result, "Controller returned null when it should have returned a response.");

        // Ensure result is of type BadRequestObjectResult
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult but got null.");
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

        // Parse response body using Newtonsoft.Json
        Assert.IsNotNull(badRequestResult.Value, "Expected response body to contain a message but got null.");
        var json = JObject.FromObject(badRequestResult.Value);
        Assert.That(json["message"]?.ToString(), Is.EqualTo("Could not build message because field Invalid email"));
    }



    

    [Test]
    public async Task SendSingleEmail_ShouldReturnNotFound_WhenTriedToFindNonExistingTemplateExceptionIsThrown()
    {
        mockEmailService.Setup(x => x.SendEmail(It.IsAny<Model_DirectEmailModel>()))
            .Throws(new TriedToFindNonExistingTemplate("Template not found"));

        var emailModel = new Model_DirectEmailModel(
            "example@example.com", "Title", "Default", "header",
            "A random body", "footer", "A name", "The person who sent"
        );

        var result = await emailController.SendSingleEmailWithTemplate(emailModel);

        AssertResponse(result, 404, "Template not found");
    }

    [Test]
    public async Task SendSingleEmail_ShouldReturnBadRequest_WhenEmailStringContainsPlaceholderExceptionIsThrown()
    {
        // Arrange
        mockEmailService.Setup(x => x.SendEmail(It.IsAny<Model_DirectEmailModel>()))
            .Throws(new EmailStringContainsPlaceholder("Placeholder issue", "Second"));

        var emailModel = new Model_DirectEmailModel(
            "example@example.com", "Title", "Default", "header",
            "A random body", "footer", "A name", "The person who sent"
        );

        // Act
        var result = await emailController.SendSingleEmailWithTemplate(emailModel);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult but got null.");
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

        var json = JObject.FromObject(badRequestResult.Value);
        Assert.IsNotNull(json["message"], "Expected response body to contain a message but got null.");
        Assert.That(json["message"]?.ToString(), Does.Contain("Placeholder issue"), "Message does not contain expected text.");
    }



    [Test]
    public async Task SendSingleEmail_ShouldReturnBadRequest_WhenTemplateRequiredFieldNotSetExceptionIsThrown()
    {
        // Arrange
        mockEmailService.Setup(x => x.SendEmail(It.IsAny<Model_DirectEmailModel>()))
            .Throws(new TemplateRequiredFieldNotSet("Required field missing"));

        var emailModel = new Model_DirectEmailModel(
            "example@example.com", "Title", "Default", "header",
            "A random body", "footer", "A name", "The person who sent"
        );

        // Act
        var result = await emailController.SendSingleEmailWithTemplate(emailModel);

        // Assert
        var badRequestResult = result as BadRequestObjectResult;
        Assert.IsNotNull(badRequestResult, "Expected BadRequestObjectResult but got null.");
        Assert.That(badRequestResult.StatusCode, Is.EqualTo(400));

        var json = JObject.FromObject(badRequestResult.Value);
        Assert.IsNotNull(json["message"], "Expected response body to contain a message but got null.");
        Assert.That(json["message"]?.ToString(), Does.Contain("Required field missing"), "Message does not contain expected text.");
    }


    [Test]
    public async Task SendSingleEmail_ShouldReturnNotFound_WhenKeyNotFoundExceptionIsThrown()
    {
        mockEmailService.Setup(x => x.SendEmail(It.IsAny<Model_DirectEmailModel>()))
            .Throws(new KeyNotFoundException("Key not found"));

        var emailModel = new Model_DirectEmailModel(
            "example@example.com", "Title", "Default", "header",
            "A random body", "footer", "A name", "The person who sent"
        );

        var result = await emailController.SendSingleEmailWithTemplate(emailModel);

        AssertResponse(result, 404, "Key not found");
    }

    private void AssertResponse(IActionResult result, int expectedStatusCode, string expectedMessage)
    {
        Assert.IsNotNull(result, "Controller returned null when it should have returned a response.");
        
        var objectResult = result as ObjectResult;
        Assert.IsNotNull(objectResult, $"Expected ObjectResult but got null.");
        Assert.That(objectResult.StatusCode, Is.EqualTo(expectedStatusCode));

        Assert.IsNotNull(objectResult.Value, "Expected response body to contain a message but got null.");
        var json = JObject.FromObject(objectResult.Value);
        Assert.That(json["message"]?.ToString(), Is.EqualTo(expectedMessage));
    }
    /// <summary>
    /// This test is for the send single email function is the function si not event meant to be used
    /// </summary>
    [Test]
    public async Task SendSingleEmail_ShouldReturnOk_WhenEmailIsSentSuccessfully()
    {
        // Arrange
        var singleEmailModel = new SingleEmailModel
        {
            // Populate with necessary properties
        };

        mockSmtpConnection
            .Setup(x => x.SendEmailAsync(It.IsAny<SingleEmailModel>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await emailController.SendSingleEmail(singleEmailModel);

        // Assert
        var okResult = result as OkObjectResult;
        Assert.IsNotNull(okResult, "Expected OkObjectResult but got null.");
        Assert.That(okResult.StatusCode, Is.EqualTo(200));
    }



}