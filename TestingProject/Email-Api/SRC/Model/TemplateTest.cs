using Email_Api.Model;

namespace TestingProject.Email_Api.SRC.Model;

[TestFixture]
public class TemplateTest
{
    [Test]
    public void Template_CanBeInitialized()
    {
        var template = new Template();
        Assert.NotNull(template);
    }

    [Test]
    public void Template_CanSetAndGetTemplateName()
    {
        var template = new Template { TemplateName = "TestTemplate" };
        Assert.That(template.TemplateName, Is.EqualTo("TestTemplate"));
    }

    [Test]
    public void Template_CanSetAndGetEmailTemplate()
    {
        var emailTemplate = new EmailTemplate("string");
        var template = new Template { EmailTemplate = emailTemplate };
        Assert.That(template.EmailTemplate, Is.EqualTo(emailTemplate));
    }
    
}