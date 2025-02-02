using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Gateway.Controller;

[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyCartController : ControllerBase
{
    private const string CartCookieName = "Cart";
    
    private readonly ServiceProductController _serviceProductController;
    private readonly ServiceCartController _serviceCartController;

    public ProxyCartController(ServiceProductController shopifyApiService, ServiceCartController cartService)
    {
        _serviceProductController = shopifyApiService;
        _serviceCartController = cartService;
    }

    [HttpGet]
    public IActionResult GetCart()
    {
        var cart = GetCartFromCookies();
        return Ok(cart);
    }

    [HttpPost("add/{productId}")]
    public async Task<IActionResult> AddToCart(long productId)
    {
        long? variantId = await _serviceCartController.GetFirstVariantId(productId);
        if (variantId == null)
        {
            return NotFound(new { Message = "Variant not found for the specified product ID." });
        }
        
        int? inventoryQuantity = await _serviceCartController.GetVariantInventoryQuantity(productId);
        if (inventoryQuantity == null || inventoryQuantity <= 0)
        {
            return BadRequest(new { Message = "Product is out of stock." });
        }

        var productData = await _serviceProductController.GetProductByIdAsync(productId);
        if (string.IsNullOrEmpty(productData) || productData.Contains("404 Not Found"))
        {
            return NotFound(new { Message = "Product not found." });
        }

        var product = JsonConvert.DeserializeObject<dynamic>(productData);
        string productTitle = product?.title;
        decimal price = product?.variants[0]?.price ?? 0;

        var cart = GetCartFromCookies();
        var cartItem = cart.FirstOrDefault(x => x.VariantId == variantId);
        if (cartItem != null)
        {
            if (cartItem.Quantity < inventoryQuantity)
            {
                cartItem.Quantity += 1;
            }
            else
            {
                return BadRequest(new { Message = "Not enough stock available." });
            }
        }
        else
        {
            cart.Add(new CartItem
            {
                ProductId = productId,
                ProductTitle = productTitle,
                VariantId = variantId.Value,
                Price = price,
                Quantity = 1
            });
        }

        SaveCartToCookies(cart);
        return Ok(cart);
    }

    [HttpPost("remove/{variantId}")]
    public IActionResult RemoveFromCart(long variantId)
    {
        var cart = GetCartFromCookies();
        var cartItem = cart.FirstOrDefault(x => x.VariantId == variantId);
        if (cartItem != null)
        {
            cart.Remove(cartItem);
            SaveCartToCookies(cart);
            return Ok(cart);
        }
        return NotFound(new { Message = "Variant not found in cart." });
    }

    [HttpPost("removebyone/{variantId}")]
    public IActionResult RemoveByOne(long variantId)
    {
        var cart = GetCartFromCookies();
        var cartItem = cart.FirstOrDefault(x => x.VariantId == variantId);
        if (cartItem != null)
        {
            cartItem.Quantity -= 1;
            if (cartItem.Quantity <= 0)
            {
                cart.Remove(cartItem);
            }
            SaveCartToCookies(cart);
            return Ok(cart);
        }
        return NotFound(new { Message = "Variant not found in cart." });
    }

    private List<CartItem> GetCartFromCookies()
    {
        if (Request.Cookies.ContainsKey(CartCookieName))
        {
            var cartJson = Request.Cookies[CartCookieName];
            if (string.IsNullOrEmpty(cartJson) || cartJson == "{}")
            {
                return new List<CartItem>();
            }

            try
            {
                return JsonConvert.DeserializeObject<List<CartItem>>(cartJson) ?? new List<CartItem>();
            }
            catch (JsonSerializationException ex)
            {
                Console.WriteLine($"Error deserializing cart: {ex.Message}");
                return new List<CartItem>();
            }
        }

        return new List<CartItem>();
    }

    private void SaveCartToCookies(List<CartItem> cart)
    {
        string cartJson = cart.Count > 0 ? JsonConvert.SerializeObject(cart) : "[]";
        Response.Cookies.Append(CartCookieName, cartJson, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}
