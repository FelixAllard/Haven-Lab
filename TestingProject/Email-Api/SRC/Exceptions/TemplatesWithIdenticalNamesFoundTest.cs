using Email_Api.Exceptions;

namespace TestingProject.Email_Api.SRC.Exceptions;

[TestFixture]
public class TemplatesWithIdenticalNamesFoundTest
{
    [Test]
    public void Exception_ShouldIncludeMessageAndTemplateName()
    {
        var exception = new TemplatesWithIdenticalNamesFound("Duplicate template: ", "TestTemplate");
        Assert.That(exception.Message, Is.EqualTo("Duplicate template: TestTemplate"));
    }
}