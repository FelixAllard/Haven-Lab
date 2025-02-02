using Api_Gateway.Services;
using Newtonsoft.Json;

namespace Api_Gateway.Services;

public class ServiceCartController
{
    private readonly ServiceProductController _serviceProductController;

    public ServiceCartController(ServiceProductController serviceProductController)
    {
        _serviceProductController = serviceProductController;
    }

    public async Task<long?> GetFirstVariantId(long productId)
    {
        try
        {
            var productJson = await _serviceProductController.GetProductByIdAsync(productId);
            if (string.IsNullOrEmpty(productJson) || productJson.Contains("404 Not Found"))
            {
                Console.WriteLine($"Error fetching product data for product ID {productId}");
                return null;
            }

            var product = JsonConvert.DeserializeObject<dynamic>(productJson);
            if (product?.variants == null || product.variants.Count == 0)
            {
                Console.WriteLine($"Error fetching variant data for product ID {productId}");
                return null;
            }

            return product.variants[0].id;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching product variants: {ex.Message}");
            return null;
        }
    }

    public async Task<int?> GetVariantInventoryQuantity(long productId)
    {
        try
        {
            Console.WriteLine($"Fetching variant data for product ID: {productId}");
            var variantJson = await _serviceProductController.GetVariantByProductIdAsync(productId);

            if (string.IsNullOrEmpty(variantJson) || variantJson.Contains("404 Not Found"))
            {
                Console.WriteLine("Variant not found in API response");
                return null;
            }

            var variants = JsonConvert.DeserializeObject<dynamic[]>(variantJson);
            if (variants == null || variants.Length == 0)
            {
                Console.WriteLine("No variants found in product response");
                return null;
            }

            Console.WriteLine($"Inventory quantity for first variant: {variants[0].inventory_quantity}");
            return variants[0].inventory_quantity ?? null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching inventory quantity: {ex.Message}");
            return null;
        }
    }
}
