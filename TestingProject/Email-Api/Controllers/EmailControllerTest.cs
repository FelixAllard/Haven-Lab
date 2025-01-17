using Email_Api.Controllers;
using Email_Api.Exceptions;
using Email_Api.Model;
using Email_Api.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

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
            // Arrange
            var singleEmailModel = new SingleEmailModel
            {
                To = "test@example.com",
                Subject = "Test Email",
                Body = "Test Body"
            };
            mockSmtpConnection.Setup(x => x.SendEmailAsync(It.IsAny<SingleEmailModel>())).Returns(Task.CompletedTask);

            // Act
            var result = await emailController.SendSingleEmail(singleEmailModel);

            // Assert
            Assert.IsInstanceOf<OkResult>(result);
            mockSmtpConnection.Verify(x => x.SendEmailAsync(singleEmailModel), Times.Once);
        }

        [Test]
        public async Task SendSingleEmailWithTemplate_ShouldReturnBadRequest_WhenBadEmailModelExceptionThrown()
        {
            // Arrange
            var directEmailModel = new DirectEmailModel();
            mockEmailService.Setup(x => x.SendEmail(It.IsAny<DirectEmailModel>())).Throws(new BadEmailModel("Invalid email"));

            // Act
            var result = await emailController.SendSingleEmailWithTemplate(directEmailModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual("Invalid email", ((dynamic)badRequestResult.Value).message);
        }

        [Test]
        public async Task SendSingleEmailWithTemplate_ShouldReturnNotFound_WhenTriedToFindNonExistingTemplateExceptionThrown()
        {
            // Arrange
            var directEmailModel = new DirectEmailModel();
            mockEmailService.Setup(x => x.SendEmail(It.IsAny<DirectEmailModel>())).Throws(new TriedToFindNonExistingTemplate("Template not found"));

            // Act
            var result = await emailController.SendSingleEmailWithTemplate(directEmailModel);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual("Template not found", ((dynamic)notFoundResult.Value).message);
        }

        [Test]
        public async Task SendSingleEmailWithTemplate_ShouldReturnBadRequest_WhenEmailStringContainsPlaceholderExceptionThrown()
        {
            // Arrange
            var directEmailModel = new DirectEmailModel();
            mockEmailService.Setup(x => x.SendEmail(It.IsAny<DirectEmailModel>())).Throws(new EmailStringContainsPlaceholder());

            // Act
            var result = await emailController.SendSingleEmailWithTemplate(directEmailModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual("Placeholder in email", ((dynamic)badRequestResult.Value).message);
        }

        [Test]
        public async Task SendSingleEmailWithTemplate_ShouldReturnBadRequest_WhenTemplateRequiredFieldNotSetExceptionThrown()
        {
            // Arrange
            var directEmailModel = new DirectEmailModel();
            mockEmailService.Setup(x => x.SendEmail(It.IsAny<DirectEmailModel>())).Throws(new TemplateRequiredFieldNotSet("Required field missing"));

            // Act
            var result = await emailController.SendSingleEmailWithTemplate(directEmailModel);

            // Assert
            var badRequestResult = result as BadRequestObjectResult;
            Assert.NotNull(badRequestResult);
            Assert.AreEqual("Required field missing", ((dynamic)badRequestResult.Value).message);
        }

        [Test]
        public async Task SendSingleEmailWithTemplate_ShouldReturnNotFound_WhenKeyNotFoundExceptionThrown()
        {
            // Arrange
            var directEmailModel = new DirectEmailModel();
            mockEmailService.Setup(x => x.SendEmail(It.IsAny<DirectEmailModel>())).Throws(new KeyNotFoundException("Key not found"));

            // Act
            var result = await emailController.SendSingleEmailWithTemplate(directEmailModel);

            // Assert
            var notFoundResult = result as NotFoundObjectResult;
            Assert.NotNull(notFoundResult);
            Assert.AreEqual("Key not found", ((dynamic)notFoundResult.Value).message);
        }

        [Test]
        public async Task SendSingleEmailWithTemplate_ShouldReturnNoContent_WhenDirectEmailModelIsNullOrEmpty()
        {
            // Arrange
            DirectEmailModel directEmailModel = null;

            // Act
            var result = await emailController.SendSingleEmailWithTemplate(directEmailModel);

            // Assert
            Assert.IsInstanceOf<NoContentResult>(result);
        }
}