using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Gateway.Controller;

[Route("gateway/api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private const string CartCookieName = "Cart";
    
    private readonly ServiceProductController _serviceProductController;

    public CartController(ServiceProductController serviceProductController)
    {
        _serviceProductController = serviceProductController;
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cart = GetCartFromCookies();

        foreach (var item in cart)
        {
            long variantId = item.Key;
            int quantity = item.Value;
            Console.WriteLine($"Variant ID: {variantId}, Quantity: {quantity}");
        }

        return Ok(cart);
    }

    [HttpPost("add/{productId}")]
    public async Task<IActionResult> AddToCart(long productId)
    {
        if (productId <= 0)
        {
            return BadRequest(new { Message = "Invalid product ID format." });
        }

        // Fetch the first variant ID for the given product ID
        long? variantId = await GetFirstVariantId(productId);
        if (variantId == null)
        {
            return NotFound(new { Message = "Variant not found for the specified product ID." });
        }

        var cart = GetCartFromCookies();
        Console.WriteLine("Current Cart: " + JsonConvert.SerializeObject(cart));

        if (cart.ContainsKey(variantId.Value))
        {
            cart[variantId.Value] += 1;
            Console.WriteLine($"Variant ID {variantId} quantity increased. New quantity: {cart[variantId.Value]}");
        }
        else
        {
            cart[variantId.Value] = 1;
            Console.WriteLine($"Variant ID {variantId} added to cart with quantity 1.");
        }

        SaveCartToCookies(cart);
        Console.WriteLine("Updated Cart: " + JsonConvert.SerializeObject(cart));
        return Ok(cart);
    }

    [HttpPost("remove/{variantId}")]
    public IActionResult RemoveFromCart(long variantId)
    {
        var cart = GetCartFromCookies();

        if (cart.ContainsKey(variantId))
        {
            cart.Remove(variantId);
            SaveCartToCookies(cart);
            return Ok(cart);
        }

        return NotFound(new { Message = "Variant not found in cart." });
    }

    [HttpPost("addbyone/{variantId}")]
    public IActionResult AddByOne(long variantId)
    {
        var cart = GetCartFromCookies();

        if (cart.ContainsKey(variantId))
        {
            cart[variantId] += 1;
            SaveCartToCookies(cart);
            Console.WriteLine($"Variant ID {variantId} quantity increased. New quantity: {cart[variantId]}");
            return Ok(cart);
        }

        return NotFound(new { Message = "Variant not found in cart." });
    }

    [HttpPost("removebyone/{variantId}")]
    public IActionResult RemoveByOne(long variantId)
    {
        var cart = GetCartFromCookies();

        if (cart.ContainsKey(variantId))
        {
            cart[variantId] -= 1;

            if (cart[variantId] <= 0)
            {
                cart.Remove(variantId);
            }

            SaveCartToCookies(cart);
            return Ok(cart);
        }

        return NotFound(new { Message = "Variant not found in cart." });
    }

    private Dictionary<long, int> GetCartFromCookies()
    {
        if (Request.Cookies.ContainsKey(CartCookieName))
        {
            var cartJson = Request.Cookies[CartCookieName];
            return JsonConvert.DeserializeObject<Dictionary<long, int>>(cartJson) ?? new Dictionary<long, int>();
        }

        return new Dictionary<long, int>();
    }

    private void SaveCartToCookies(Dictionary<long, int> cart)
    {
        var cartJson = JsonConvert.SerializeObject(cart);
        Response.Cookies.Append(CartCookieName, cartJson, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }

    private async Task<long?> GetFirstVariantId(long productId)
    {
        try
        {
            var productControllerUrl = $"http://localhost:5158/gateway/api/proxyproduct/variant/{productId}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(productControllerUrl);

            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Error fetching product data for product ID {productId}: {errorContent}");
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response content: {content}");

            var variantData = JsonConvert.DeserializeObject<dynamic>(content);

            if (variantData == null || variantData.variantId == null)
            {
                Console.WriteLine($"Variant ID not found for product ID {productId}");
                return null;
            }

            long variantId = variantData.variantId;
            Console.WriteLine($"First variant ID for product ID {productId}: {variantId}");
            return variantId;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching first variant ID: {ex.Message}");
            return null;
        }
    }

}
