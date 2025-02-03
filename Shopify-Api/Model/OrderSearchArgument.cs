namespace Shopify_Api.Model;

public class OrderSearchArgument
{
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime? DateBefore { get; set; } = null;
    public DateTime? DateAfter { get; set; } = null;
}