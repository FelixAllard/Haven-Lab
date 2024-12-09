using ShopifySharp.GraphQL;

namespace TestingProject.Utility;

public class ProductComparer : IEqualityComparer<Product>
{
    public bool Equals(Product x, Product y)
    {
        return x.id == y.id && x.title == y.title;
    }

    public int GetHashCode(Product obj)
    {
        return HashCode.Combine(obj.id, obj.title);
    }
}
