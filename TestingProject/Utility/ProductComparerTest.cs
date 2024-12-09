using ShopifySharp;

namespace TestingProject.Utility;

[TestFixture]
public class ProductComparerTest
{
    // Test when two Products have the same id and Title
    [Test]
    public void Equals_SameIdAndTitle_ReturnsTrue()
    {
        var product1 = new Product { Id = 1, Title = "Test Product" };
        var product2 = new Product { Id = 1, Title = "Test Product" };

        var comparer = new ProductComparer();

        bool result = comparer.Equals(product1, product2);

        Assert.That(result, Is.True);
    }

    // Test when two Products have different Id
    [Test]
    public void Equals_DifferentId_ReturnsFalse()
    {
        var product1 = new Product { Id = 1, Title = "Test Product" };
        var product2 = new Product { Id = 2, Title = "Test Product" };

        var comparer = new ProductComparer();

        bool result = comparer.Equals(product1, product2);

        Assert.That(result, Is.False);
    }

    // Test when two Products have different Titles
    [Test]
    public void Equals_DifferentTitle_ReturnsFalse()
    {
        var product1 = new Product { Id = 1, Title = "Test Product" };
        var product2 = new Product { Id = 1, Title = "Different Product" };

        var comparer = new ProductComparer();

        bool result = comparer.Equals(product1, product2);

        Assert.That(result, Is.False);
    }

    // Test when GetHashCode is called with same id and Title
    [Test]
    public void GetHashCode_SameIdAndTitle_ReturnsSameHashCode()
    {
        var product1 = new Product { Id = 1, Title = "Test Product" };
        var product2 = new Product { Id = 1, Title = "Test Product" };

        var comparer = new ProductComparer();

        int hashCode1 = comparer.GetHashCode(product1);
        int hashCode2 = comparer.GetHashCode(product2);

        Assert.That(hashCode1, Is.EqualTo(hashCode2));
    }

    // Test when GetHashCode is called with different id or Title
    [Test]
    public void GetHashCode_DifferentIdOrTitle_ReturnsDifferentHashCode()
    {
        var product1 = new Product { Id = 1, Title = "Test Product" };
        var product2 = new Product { Id = 2, Title = "Test Product" };

        var comparer = new ProductComparer();

        int hashCode1 = comparer.GetHashCode(product1);
        int hashCode2 = comparer.GetHashCode(product2);

        Assert.That(hashCode1, Is.Not.EqualTo(hashCode2));
    }

    // Test when Equals is called with null
    /*[Test]
    public void Equals_WithNull_ReturnsFalse()
    {
        var product1 = new Product { Id = 1, Title = "Test Product" };
        Product product2 = null;

        var comparer = new ProductComparer();

        bool result = comparer.Equals(product1, product2);

        Assert.That(result, Is.False);
    }*/

    // Test when Equals is called with the same object
    [Test]
    public void Equals_SameObject_ReturnsTrue()
    {
        var product = new Product { Id = 1, Title = "Test Product" };

        var comparer = new ProductComparer();

        bool result = comparer.Equals(product, product);

        Assert.That(result, Is.True);
    }
}