using Email_Api;

namespace TestingProject.Email_Api.SRC;

[TestFixture]
public class EmailUtilsTest
{
    [TestCase("example@gmail.com")]
    [TestCase("example@gmail.qc.com")]
    [TestCase("e@gmail.qc.com")]
    [Test]
    public async Task CheckIfEmailIsValid_ShouldReturnTrue_WhenValidEmailIsPassed(string email)
    {
        Assert.IsTrue(EmailUtils.CheckIfEmailIsValid(email));
    }
    [TestCase("@gmail.com")]
    [TestCase("example@gmai@l.qc.com")]
    [TestCase("e@gmail")]
    [Test]
    public async Task CheckIfEmailIsValid_ShouldReturnFalse_WhenInvalidEmailIsPassed(string email)
    {
        Assert.IsFalse(EmailUtils.CheckIfEmailIsValid(email));
    }
    
    [TestCase("")]
    [TestCase(null)]
    [Test]
    public async Task CheckIfEmailIsValid_ShouldReturnFalse_WhenEmptyStringOrNullStringIsPassed(string? email)
    {
        Assert.IsFalse(EmailUtils.CheckIfEmailIsValid(email));
    }
    
}