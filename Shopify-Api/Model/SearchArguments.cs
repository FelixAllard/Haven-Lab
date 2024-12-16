namespace Shopify_Api.Model;

public class SearchArguments
{
    public string Name { get; set; } = string.Empty;
    public long MinimumPrice { get; set; } = long.MinValue;
    public long MaximumPrice { get; set; } = long.MaxValue;
    public bool Available { get; set; } = false;
}