using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ApiWebAuth.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;

namespace ApiWebAuth.Controllers;
[Route("api/[controller]")]
[ApiController]
public class AccountController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public AccountController(UserManager<IdentityUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] Register model)
    {
        var user = new IdentityUser { UserName = model.Username };
        var result = await _userManager.CreateAsync(user, model.Password);

        if (result.Succeeded)
        {
            return Ok("Registration successful");
        }
        return BadRequest(result.Errors);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] Login model)
    {
        var user = await _userManager.FindByNameAsync(model.Username);
        if (user != null && await _userManager.CheckPasswordAsync(user, model.Password))
        {
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(authClaims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(_configuration["Jwt:ExpiryMinutes"]!)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = _configuration["Jwt:Issuer"]
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);
            
            await _userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", tokenString);

            return Ok(new { token = tokenString });
        }
        return Unauthorized();
    }
    
    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string username)
    {
        var user = await _userManager.FindByNameAsync(username);
        if (user == null)
            return NotFound("User not found.");

        await _userManager.RemoveAuthenticationTokenAsync(user, "JWT", "AccessToken");

        return Ok("User logged out successfully.");
    }
    
        
    [HttpPost("verify-token")]
    public async Task<IActionResult> VerifyToken([FromBody] string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();

        try
        {
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!);

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _configuration["Jwt:Issuer"],
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            };

            var principal = tokenHandler.ValidateToken(token, validationParameters, out SecurityToken validatedToken);
            var jwtToken = validatedToken as JwtSecurityToken;

            if (jwtToken == null)
                return Unauthorized("Invalid token format.");
            

            var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value 
                           ?? principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized("Invalid token claims.");
            }
            
            if (string.IsNullOrEmpty(username))
                return Unauthorized("Invalid token claims.");
            

            // Check if token is stored in the database
            var user = await _userManager.FindByNameAsync(username);
            if (user == null)
                return Unauthorized("User not found.");

            var storedToken = await _userManager.GetAuthenticationTokenAsync(user, "JWT", "AccessToken");

            if (storedToken == token)
                return Ok(new { message = "Token is valid" });
            else
                return Unauthorized("Token mismatch.");
        }
        catch (SecurityTokenExpiredException)
        {
            return Unauthorized("Token has expired.");
        }
        catch (Exception ex)
        {
            return BadRequest($"Token validation failed: {ex.Message}");
        }
    }


}