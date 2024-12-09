using ShopifySharp;

namespace TestingProject.Utility;

public class ProductComparer : IEqualityComparer<Product>
{
    public bool Equals(Product x, Product y)
    {
        return x.Id == y.Id && x.Title == y.Title;
    }

    public int GetHashCode(Product obj)
    {
        return HashCode.Combine(obj.Id, obj.Title);
    }
}
