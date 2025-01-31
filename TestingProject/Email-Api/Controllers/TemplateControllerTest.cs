using Email_Api.Controllers;
using Email_Api.Exceptions;
using Email_Api.Model;
using Email_Api.Service;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace TestingProject.Email_Api.Controllers;

[TestFixture]
    public class TemplateControllerTests
    {
        private Mock<ITemplateService> _mockTemplateService;
        private TemplateController _controller;

        [SetUp]
        public void Setup()
        {
            _mockTemplateService = new Mock<ITemplateService>();
            _controller = new TemplateController(_mockTemplateService.Object);
        }

        [Test]
        public async Task GetAllTemplatesNames_ReturnsOk()
        {
            _mockTemplateService.Setup(s => s.GetAllTemplatesNames()).Returns(new List<string> { "Template1", "Template2" });

            var result = await _controller.GetAllTemplatesNames();
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
        }

        [Test]
        public async Task GetTemplateByName_ReturnsOk_WhenTemplateExists()
        {
            var template = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            _mockTemplateService.Setup(s => s.GetTemplateByName("TestTemplate")).Returns(template);

            var result = await _controller.GetTemplateByName("TestTemplate") as OkObjectResult;
            Assert.NotNull(result);
            Assert.That(result.Value, Is.EqualTo(template));
        }

        [Test]
        public async Task GetTemplateByName_ReturnsNotFound_WhenTemplateDoesNotExist()
        {
            _mockTemplateService.Setup(s => s.GetTemplateByName("NonExistentTemplate")).Throws(new KeyNotFoundException());

            var result = await _controller.GetTemplateByName("NonExistentTemplate") as NotFoundObjectResult;
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task GetAllTemplates_ReturnsOk_WhenNoExceptionThrown()
        {
            var template = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            var template2 = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            List<Template> templates = new List<Template>
            {
                template,
                template2,
            };
            _mockTemplateService.Setup(s => s.GetAllTemplates()).Returns(templates);
            var result = await _controller.GetAllTemplates() as OkObjectResult;
            Assert.NotNull(result);
            Assert.That(result, Is.InstanceOf<OkObjectResult>());
            Assert.That(result.Value, Is.EqualTo(templates));
            
        }

        [Test]
        public async Task PostTemplate_ReturnsOk_WhenTemplateIsAdded()
        {
            var template = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            _mockTemplateService.Setup(s => s.PostTemplate(template)).Returns(template);

            var result = await _controller.PostTemplate(template) as OkObjectResult;
            Assert.NotNull(result);
            Assert.That(result.Value, Is.EqualTo(template));
        }

        [Test]
        public async Task PostTemplate_ReturnsBadRequest_WhenDuplicateTemplatesExist()
        {
            var template = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            _mockTemplateService.Setup(s => s.PostTemplate(template)).Throws(new TemplatesWithIdenticalNamesFound("Duplicate error", template.TemplateName));

            var result = await _controller.PostTemplate(template) as BadRequestObjectResult;
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(400));
        }

        [Test]
        public async Task PutTemplate_ReturnsOk_WhenTemplateIsUpdated()
        {
            var template = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            _mockTemplateService.Setup(s => s.PutTemplate("UpdatedTemplate", template)).Returns(template);

            var result = await _controller.PutTemplate("UpdatedTemplate", template) as OkObjectResult;
            Assert.NotNull(result);
            Assert.That(result.Value, Is.EqualTo(template));
        }

        [Test]
        public async Task PutTemplate_ReturnsNotFound_WhenDefaultTemplateIsModified()
        {
            string defaultTemplateName = "Default";
            var template = new Template { TemplateName = "Default", EmailTemplate = new EmailTemplate("Example") };
            _mockTemplateService.Setup(s => s.PutTemplate(defaultTemplateName, template)).Throws(new UnauthorizedAccessException());

            var result = await _controller.PutTemplate(defaultTemplateName, template) as UnauthorizedObjectResult;
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(401));
        }
        [Test]
        public async Task PutTemplate_ReturnsNotFound_WhenTemplateDoesNotExist()
        {
            var template = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            _mockTemplateService.Setup(s => s.PutTemplate("NonExistentTemplate", template)).Throws(new KeyNotFoundException());

            var result = await _controller.PutTemplate("NonExistentTemplate", template) as NotFoundObjectResult;
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }

        [Test]
        public async Task DeleteTemplate_ReturnsOk_WhenTemplateIsDeleted()
        {
            var template = new Template { TemplateName = "TestTemplate", EmailTemplate = new EmailTemplate("Example") };
            _mockTemplateService.Setup(s => s.DeleteTemplate("TemplateToDelete")).Returns(template);

            var result = await _controller.DeleteTemplate("TemplateToDelete") as OkObjectResult;
            Assert.NotNull(result);
            Assert.That(result.Value, Is.EqualTo(template));
        }

        [Test]
        public async Task DeleteTemplate_ReturnsNotFound_WhenTemplateDoesNotExist()
        {
            _mockTemplateService.Setup(s => s.DeleteTemplate("NonExistentTemplate")).Throws(new KeyNotFoundException());

            var result = await _controller.DeleteTemplate("NonExistentTemplate") as NotFoundObjectResult;
            Assert.NotNull(result);
            Assert.That(result.StatusCode, Is.EqualTo(404));
        }
    }