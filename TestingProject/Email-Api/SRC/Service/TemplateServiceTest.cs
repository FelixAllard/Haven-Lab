using Email_Api.Exceptions;
using Email_Api.Model;
using Email_Api.Service;
using Moq;

namespace TestingProject.Email_Api.SRC.Service;

[TestFixture]
public class TemplateServiceTest
{
    private Mock<ITemplateManager> _mockTemplateManager;
    private TemplateService _templateService;

    [SetUp]
    public void Setup()
    {
        _mockTemplateManager = new Mock<ITemplateManager>();
        _templateService = new TemplateService(_mockTemplateManager.Object);
    }

    [Test]
    public void GetAllTemplatesNames_ReturnsCorrectNames()
    {
        var templates = new List<Template>
        {
            new Template { TemplateName = "Template1" },
            new Template { TemplateName = "Template2" }
        };
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(templates);

        var result = _templateService.GetAllTemplatesNames();

        Assert.That(result, Is.EqualTo(new List<string> { "Template1", "Template2" }));
    }

    [Test]
    public void GetAllTemplates_ReturnsTemplatesList()
    {
        var templates = new List<Template> { new Template(), new Template() };
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(templates);

        var result = _templateService.GetAllTemplates();

        Assert.That(result, Is.EqualTo(templates));
    }

    [Test]
    public void GetTemplateByName_ReturnsCorrectTemplate()
    {
        var emailTemplate = new EmailTemplate("SomeRandomHtml");
        _mockTemplateManager.Setup(m => m.GetTemplate("TestTemplate")).Returns(emailTemplate);

        var result = _templateService.GetTemplateByName("TestTemplate");

        Assert.That(result.TemplateName, Is.EqualTo("TestTemplate"));
        Assert.That(result.EmailTemplate, Is.EqualTo(emailTemplate));
    }

    [Test]
    public void PostTemplate_ThrowsException_WhenTemplateAlreadyExists()
    {
        var template = new Template { TemplateName = "ExistingTemplate" };
        var existingTemplates = new List<Template> { new Template { TemplateName = "ExistingTemplate" } };
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(existingTemplates);

        Assert.Throws<TemplatesWithIdenticalNamesFound>(() => _templateService.PostTemplate(template));
    }

    [Test]
    public void PostTemplate_AddsTemplateSuccessfully()
    {
        var template = new Template { TemplateName = "NewTemplate" };
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(new List<Template>());
        _mockTemplateManager.Setup(m => m.PostTemplate(template)).Returns(template);

        var result = _templateService.PostTemplate(template);

        Assert.That(result, Is.EqualTo(template));
    }

    [Test]
    public void PutTemplate_ThrowsException_WhenModifyingDefaultTemplate()
    {
        var template = new Template { TemplateName = "Default" };
        Assert.Throws<UnauthorizedAccessException>(() => _templateService.PutTemplate("Default", template));
    }

    [Test]
    public void PutTemplate_ThrowsException_WhenTemplateDoesNotExist()
    {
        var template = new Template { TemplateName = "NonExistent" };
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(new List<Template>());

        Assert.Throws<KeyNotFoundException>(() => _templateService.PutTemplate("NonExistent", template));
    }

    [Test]
    public void PutTemplate_UpdatesExistingTemplate()
    {
        var template = new Template { TemplateName = "ExistingTemplate" };
        var existingTemplates = new List<Template> { new Template { TemplateName = "ExistingTemplate" } };
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(existingTemplates);
        _mockTemplateManager.Setup(m => m.PutTemplate("ExistingTemplate", template)).Returns(template);

        var result = _templateService.PutTemplate("ExistingTemplate", template);

        Assert.That(result, Is.EqualTo(template));
    }

    [Test]
    public void DeleteTemplate_ThrowsException_WhenDeletingDefaultTemplate()
    {
        Assert.Throws<UnauthorizedAccessException>(() => _templateService.DeleteTemplate("Default"));
    }

    [Test]
    public void DeleteTemplate_ThrowsException_WhenTemplateDoesNotExist()
    {
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(new List<Template>());

        Assert.Throws<KeyNotFoundException>(() => _templateService.DeleteTemplate("NonExistent"));
    }

    [Test]
    public void DeleteTemplate_DeletesExistingTemplate()
    {
        var template = new Template { TemplateName = "TemplateToDelete" };
        var existingTemplates = new List<Template> { template };
        _mockTemplateManager.Setup(m => m.GetTemplates()).Returns(existingTemplates);
        _mockTemplateManager.Setup(m => m.DeleteTemplate("TemplateToDelete")).Returns(template);

        var result = _templateService.DeleteTemplate("TemplateToDelete");

        Assert.That(result, Is.EqualTo(template));
    }
}