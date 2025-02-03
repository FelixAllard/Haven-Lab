namespace Api_Gateway.Models;

public class CartItem
{
    public long ProductId { get; set; }
    public string ProductTitle { get; set; }
    public long VariantId { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
