using Api_Gateway.Models;
using Api_Gateway.Services;
using Newtonsoft.Json;

namespace Api_Gateway.Services;

public class ServiceCartController
{
    private const string CartCookieName = "Cart";
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ServiceCartController(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public virtual List<CartItem> GetCartFromCookies()
    {
        var request = _httpContextAccessor.HttpContext.Request;
        
        if (request.Cookies.ContainsKey(CartCookieName))
        {
            var cartJson = request.Cookies[CartCookieName];
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

    public virtual void SaveCartToCookies(List<CartItem> cart)
    {
        var response = _httpContextAccessor.HttpContext?.Response;
        
        string cartJson = cart.Count > 0 ? JsonConvert.SerializeObject(cart) : "[]";
        response.Cookies.Append(CartCookieName, cartJson, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UtcNow.AddDays(7)
        });
    }
}
