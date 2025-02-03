using System.Diagnostics;
using Shopify_Api.Exceptions;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Filters;

namespace Shopify_Api.Controllers;

using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ShopifySharp;

[ApiController]
[Route("api/[controller]")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _shopifyService;

    public CustomerController(
        ICustomerServiceFactory productServiceFactory,
        Shopify_Api.ShopifyRestApiCredentials credentials
        )
    {
        _shopifyService = productServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );
        
    }

    [HttpPost("subscribe")]
    public async Task<IActionResult> Subscribe([FromBody] string email)
    {
        try
        {   
            var existingCustomers = await GetCustomersByEmailAsync(email);
            Console.WriteLine("Customers being updated:");
            foreach (var customer in existingCustomers)
            {
                Console.WriteLine($"Customer Email: {customer.Email}");
            }

            if (!existingCustomers.Any())
            {
                // If no customer exists, create a new one
                var newCustomer = new Customer
                {
                    Email = email,
                    Tags = "Subscribed",
                };
                await _shopifyService.CreateAsync(newCustomer);
            }
            else
            {
                // If customer exists, update their marketing preference
                var customer = existingCustomers.First();

                customer.Tags = "Subscribed";
                await _shopifyService.UpdateAsync(customer.Id.Value, customer);
            }
            return Ok(new { message = "Subscribed successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while subscribing.", error = ex.Message });
        }
    }

    private async Task<IEnumerable<Customer>> GetCustomersByEmailAsync(string email)
    {
        var result = await _shopifyService.ListAsync(new CustomerListFilter());

        return result.Items.Where(c => c.Email != null && c.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }

    
    [HttpPost("unsubscribe")]
    public async Task<IActionResult> Unsubscribe([FromBody] string email)
    {
        try
        {
            // Search for existing customers by email using a custom method
            var existingCustomers = await GetCustomersByEmailAsync(email);

            if (!existingCustomers.Any())
            {
                return NotFound(new { message = "Customer not found." });
            }
            
            var customer = existingCustomers.First();

            customer.Tags = customer.Tags.Replace("Subscribed", "").Trim();
            
            await _shopifyService.UpdateAsync(customer.Id.Value, customer);

            return Ok(new { message = "Unsubscribed successfully!" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while unsubscribing.", error = ex.Message });
        }
    }

}
