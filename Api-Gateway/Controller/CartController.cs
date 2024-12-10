using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Gateway.Controller;
[Route("gateway/api/[controller]")]
[ApiController]
public class CartController : ControllerBase
{
    private const string CartCookieName = "Cart";
    
    [HttpGet]
    public IActionResult GetCart()
    {
        var cart = GetCartFromCookies();
        return Ok(cart);
    }
    
    [HttpPost("add/{productId}")]
    public async Task<IActionResult> AddToCart(long productId)
    {
        if (productId <= 0)
        {
            return BadRequest(new { Message = "Invalid product ID format." });
        }
        
        if (!await IsValidProductId(productId))
        {
            return NotFound(new { Message = "Invalid product ID." });
        }

        var cart = GetCartFromCookies();

        if (cart.ContainsKey(productId))
        {
            cart[productId] += 1; //add one quantity if product is already in cart
        }
        else
        {
            cart[productId] = 1; 
        }

        SaveCartToCookies(cart);
        return Ok(cart);
    }

    
    [HttpPost("remove/{productId}")]
    public IActionResult RemoveFromCart(long productId) 
    {
        var cart = GetCartFromCookies();

        if (cart.ContainsKey(productId))
        {
            cart.Remove(productId);
            SaveCartToCookies(cart);
            return Ok(cart);
        }

        return NotFound(new { Message = "Product not found in cart." });
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
    
    private async Task<bool> IsValidProductId(long productId)
    {
        try
        {
            var productControllerUrl = $"http://localhost:5158/gateway/api/proxyproduct/{productId}";

            using var httpClient = new HttpClient();
            var response = await httpClient.GetAsync(productControllerUrl);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }

            var errorContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Validation error: {errorContent}");
            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error validating product ID: {ex.Message}");
            return false;
        }
    }
}
