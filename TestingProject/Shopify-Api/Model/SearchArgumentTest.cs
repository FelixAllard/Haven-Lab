using Shopify_Api.Model;

namespace TestingProject.Shopify_Api.Model;
[TestFixture]
public class SearchArgumentTest
{
    [Test]
    public void SearchArguments_DefaultValues_AreSetCorrectly()
    {
        // Arrange & Act
        var searchArgs = new SearchArguments();

        // Assert
        Assert.That(searchArgs.Name, Is.EqualTo(string.Empty));
        Assert.That(searchArgs.MinimumPrice, Is.EqualTo(decimal.MinValue));
        Assert.That(searchArgs.MaximumPrice, Is.EqualTo(decimal.MaxValue));
        Assert.That(searchArgs.Available, Is.False);
    }

    [Test]
    public void SearchArguments_CanSetName()
    {
        // Arrange
        var searchArgs = new SearchArguments();
        var testName = "Test Product";

        // Act
        searchArgs.Name = testName;

        // Assert
        Assert.That(searchArgs.Name, Is.EqualTo(testName));
    }

    [Test]
    public void SearchArguments_CanSetMinimumPrice()
    {
        // Arrange
        var searchArgs = new SearchArguments();
        long testMinPrice = 100;

        // Act
        searchArgs.MinimumPrice = testMinPrice;

        // Assert
        Assert.That(searchArgs.MinimumPrice, Is.EqualTo(testMinPrice));
    }

    [Test]
    public void SearchArguments_CanSetMaximumPrice()
    {
        // Arrange
        var searchArgs = new SearchArguments();
        long testMaxPrice = 500;

        // Act
        searchArgs.MaximumPrice = testMaxPrice;

        // Assert
        Assert.That(searchArgs.MaximumPrice, Is.EqualTo(testMaxPrice));
    }

    [Test]
    public void SearchArguments_CanSetAvailable()
    {
        // Arrange
        var searchArgs = new SearchArguments();

        // Act
        searchArgs.Available = true;

        // Assert
        Assert.That(searchArgs.Available, Is.True);
    }
}
