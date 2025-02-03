using Api_Gateway.Models;
using Api_Gateway.Services;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Api_Gateway.Controller;

[Route("gateway/api/[controller]")]
[ApiController]
public class ProxyCartController : ControllerBase
{
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
        var cart = _serviceCartController.GetCartFromCookies();
        return Ok(cart);
    }

    [HttpPost("add/{productId}")]
    public async Task<IActionResult> AddToCart(long productId)
    {
        var productData = await _serviceProductController.GetProductByIdAsync(productId);
        if (string.IsNullOrEmpty(productData) || productData.Contains("404 Not Found"))
        {
            return NotFound(new { Message = "Product not found." });
        }

        var product = JsonConvert.DeserializeObject<dynamic>(productData);
        string productTitle = product?.title;
        decimal price = product?.variants[0]?.price ?? 0;
        long? variantId = product?.variants[0]?.id;
        int? inventoryQuantity = product?.variants[0]?.inventory_quantity ?? 0;
        
        if (inventoryQuantity <= 0)
        {
            return BadRequest(new { Message = "Product is out of stock." });
        }

        var cart = _serviceCartController.GetCartFromCookies();
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

        _serviceCartController.SaveCartToCookies(cart);
        return Ok(cart);
    }

    [HttpPost("remove/{variantId}")]
    public IActionResult RemoveFromCart(long variantId)
    {
        var cart = _serviceCartController.GetCartFromCookies();
        var cartItem = cart.FirstOrDefault(x => x.VariantId == variantId);
        if (cartItem != null)
        {
            cart.Remove(cartItem);
            _serviceCartController.SaveCartToCookies(cart);
            return Ok(cart);
        }
        return NotFound(new { Message = "Variant not found in cart." });
    }

    [HttpPost("addbyone/{variantId}")]
    public async Task<IActionResult> AddByOne(long variantId)
    {
        var cart = _serviceCartController.GetCartFromCookies();
        var cartItem = cart.FirstOrDefault(x => x.VariantId == variantId);
        if (cartItem != null)
        {
            var productData = await _serviceProductController.GetProductByIdAsync(cartItem.ProductId);
            if (string.IsNullOrEmpty(productData) || productData.Contains("404 Not Found"))
            {
                return NotFound(new { Message = "Product not found." });
            }

            var product = JsonConvert.DeserializeObject<dynamic>(productData);
            int? inventoryQuantity = product?.variants[0]?.inventory_quantity ?? 0;

            if (cartItem.Quantity < inventoryQuantity)
            {
                cartItem.Quantity += 1;
                _serviceCartController.SaveCartToCookies(cart);
                return Ok(cart);
            }
            else
            {
                return BadRequest(new { Message = "Not enough stock available." });
            }
        }
        return NotFound(new { Message = "Variant not found in cart." });
    }
    
    [HttpPost("removebyone/{variantId}")]
    public IActionResult RemoveByOne(long variantId)
    {
        var cart = _serviceCartController.GetCartFromCookies();
        var cartItem = cart.FirstOrDefault(x => x.VariantId == variantId);
        if (cartItem != null)
        {
            cartItem.Quantity -= 1;
            if (cartItem.Quantity <= 0)
            {
                cart.Remove(cartItem);
            }
            _serviceCartController.SaveCartToCookies(cart);
            return Ok(cart);
        }
        return NotFound(new { Message = "Variant not found in cart." });
    }
    
}
