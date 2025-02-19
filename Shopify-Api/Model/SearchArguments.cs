namespace Shopify_Api.Model;

public class SearchArguments
{
    public string Name { get; set; } = string.Empty;
    public decimal MinimumPrice { get; set; } = decimal.MinValue;
    public decimal MaximumPrice { get; set; } = decimal.MaxValue;
    public bool Available { get; set; } = false;
}