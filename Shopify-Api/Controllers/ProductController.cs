using System.Text;
using System.Xml.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Shopify_Api.Exceptions;
using Shopify_Api.Model;
using ShopifySharp;
using ShopifySharp.Factories;
using ShopifySharp.Lists;
using Product = ShopifySharp.Product;
using Microsoft.AspNetCore.Mvc;

namespace Shopify_Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _shopifyService;
    private readonly ProductValidator _productValidator;
    private readonly IMetaFieldService _metaFieldService;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ShopifyRestApiCredentials _credentials;
    private readonly string _shopifyStoreName = "vc-shopz";
    private readonly string _apiVersion = "2023-10";

    public ProductsController(
        IProductServiceFactory productServiceFactory,
        ShopifyRestApiCredentials credentials,
        ProductValidator productValidator,
        IMetaFieldServiceFactory metaFieldServiceFactory,
        IHttpClientFactory httpClientFactory
    )
    {
        _credentials = credentials;
        _shopifyService = productServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );

        _metaFieldService = metaFieldServiceFactory.Create(new ShopifySharp.Credentials.ShopifyApiCredentials(
                credentials.ShopUrl,
                credentials.AccessToken
            )
        );

        _productValidator = productValidator;
        _httpClientFactory = httpClientFactory;
    }

    //================================ PRODUCT ENDPOINTS ==================================

    [HttpGet("")]
    public async Task<IActionResult> GetAllProducts([FromQuery] SearchArguments searchArguments = null)
    {
        try
        {
            var products = await _shopifyService.ListAsync();

            if (searchArguments == null)
            {
                return Ok(products);
            }

            var filteredItems = products.Items.AsQueryable();

            // Filter by name if provided
            if (!string.IsNullOrWhiteSpace(searchArguments.Name))
            {
                filteredItems = filteredItems.Where(x => x.Title.ToLower().Contains(searchArguments.Name.ToLower()));
            }

            // Filter by minimum price if provided
            if (searchArguments.MinimumPrice > 0)
            {
                filteredItems =
                    filteredItems.Where(x => x.Variants.FirstOrDefault().Price >= searchArguments.MinimumPrice);
            }

            // Filter by maximum price if provided
            if (searchArguments.MaximumPrice > 0)
            {
                filteredItems =
                    filteredItems.Where(x => x.Variants.FirstOrDefault().Price <= searchArguments.MaximumPrice);
            }

            // Filter by availability if specified
            if (searchArguments.Available)
            {
                filteredItems = filteredItems.Where(x => x.Variants.FirstOrDefault().InventoryQuantity > 0);
            }

            return Ok(new ListResult<Product>(filteredItems.ToList(), null));
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "Error fetching product", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching product", details = ex.Message });
        }
    }


    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById([FromRoute] long id)
    {
        try
        {
            Console.WriteLine(id);
            var products = await _shopifyService.GetAsync(id);
            return Ok(products);
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "Error fetching products", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching products" + ex.Message });
        }
    }

    [HttpPost("")]
    public virtual async Task<IActionResult> PostProduct([FromBody] Product product)
    {
        try
        {
            Product tempProduct = _productValidator.FormatPostProduct(product);
            Console.Write("We formatted!");
            var products = await _shopifyService.CreateAsync(tempProduct);
            return Ok(products);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching products", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { message = "Error creating product " + ex.Message });
        }
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> PutProduct([FromRoute] long id, [FromBody] Product product)
    {
        try
        {
            //Product tempProduct = _productValidator.FormatPostProduct(product);
            //Console.Write("We formatted!");
            var products = await _shopifyService.UpdateAsync(id, product);
            return Ok(products);
        }
        catch (InputException ex)
        {
            return StatusCode(400, new { message = ex.Message });
        }
        catch (ShopifyException ex)
        {
            return StatusCode(500, new { message = "Error fetching products", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            // Log the exception if necessary
            return StatusCode(500, new { message = "Error updating product " + ex.Message });
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct([FromRoute] long id)
    {
        try
        {
            Console.WriteLine(id);
            await _shopifyService.DeleteAsync(id);
            return Ok("Product deleted");
        }
        catch (ShopifyException ex)
        {
            return StatusCode(404, new { message = "No product found", details = ex.Message });
        }
        catch (System.Exception ex)
        {
            return StatusCode(500, new { message = "Error deleting products" + ex.Message });
        }
    }

    [HttpGet("variant/{productId}")]
    public async Task<IActionResult> GetFirstVariantByProductId([FromRoute] long productId)
    {
        try
        {
            //fetch product using the Shopify service
            var product = await _shopifyService.GetAsync(productId);

            if (product == null || product.Variants == null || !product.Variants.Any())
            {
                return NotFound(new { message = "Product not found or no variants available." });
            }

            var firstVariant = product.Variants.FirstOrDefault();

            long firstVariantId = firstVariant.Id.Value;

            return Ok(new { VariantId = firstVariantId });
        }

        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching product variants", details = ex.Message });
        }
    }

    //================================ TRANSLATED METAFIELD ENDPOINTS ==================================

    [HttpGet("{id}/translation")]
    public async Task<IActionResult> GetTranslatedProduct([FromRoute] long id, [FromQuery] string lang = "fr")
    {
        try
        {
            var metafields = await _metaFieldService.ListAsync(id, "products");

            var titleMetafield =
                metafields?.Items?.FirstOrDefault(m => m.Key == $"title_{lang}" && m.Namespace == "translations");
            var descriptionMetafield =
                metafields?.Items?.FirstOrDefault(m => m.Key == $"description_{lang}" && m.Namespace == "translations");

            string translatedTitle = titleMetafield?.Value?.ToString() ?? "N/A";
            string translatedDescription = descriptionMetafield?.Value?.ToString() ?? "N/A";

            return Ok(new
            {
                ProductId = id,
                Title = translatedTitle,
                Description = translatedDescription
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error fetching translations", details = ex.Message });
        }
    }

    [HttpPost("{id}/translation")]
    public async Task<IActionResult> AddProductTranslation([FromRoute] long id, [FromBody] TranslationRequest request)
    {
        try
        {
            var metafieldTitle = new MetaField()
            {
                Namespace = "translations",
                Key = $"title_{request.Locale}",
                Value = request.Title,
                Type = "string",
                OwnerId = id,
                OwnerResource = "product"
            };

            var metafieldDescription = new MetaField()
            {
                Namespace = "translations",
                Key = $"description_{request.Locale}",
                Value = request.Description,
                Type = "string",
                OwnerId = id,
                OwnerResource = "product"
            };

            await _metaFieldService.CreateAsync(metafieldTitle, id, "products");
            await _metaFieldService.CreateAsync(metafieldDescription, id, "products");

            return Ok(new { message = "Translation added!", productId = id });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error saving translation", details = ex.Message });
        }
    }

    [HttpPost("upload-image")]
    public async Task<IActionResult> UploadImage([FromBody] byte[] imageData)
    {
        if (imageData == null || imageData.Length == 0)
        {
            return BadRequest("No image data provided.");
        }

        // Print image data for debugging (avoid for large images in production)
        Console.WriteLine($"Received image with {imageData.Length} bytes.");

        var imageUrl = await UploadImageToShopify(imageData);

        if (string.IsNullOrEmpty(imageUrl))
        {
            return StatusCode(500, "Failed to upload image to Shopify.");
        }

        return Ok(new { ImageUrl = imageUrl });
    }

    public class Parameter
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }
    
    private async Task<string> UploadImageToShopify(byte[] imageData)
    {
        var graphqlUrl = $"https://{_shopifyStoreName}.myshopify.com/admin/api/{_apiVersion}/graphql.json";
        var client = _httpClientFactory.CreateClient();
        string locationURL;

        // Prepare the GraphQL query to generate a staged upload URL
        var query = @"
            mutation generateStagedUploads {
              stagedUploadsCreate(input: [
                {
                  filename: ""product-image.jpg"",
                  mimeType: ""image/jpeg"",
                  resource: IMAGE,
                  httpMethod: POST
                }
              ]) {
                stagedTargets {
                  url
                  resourceUrl
                  parameters {
                    name
                    value
                  }
                }
                userErrors {
                  field
                  message
                }
              }
            }";

        var request = new HttpRequestMessage(HttpMethod.Post, graphqlUrl)
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(new { query }),
                Encoding.UTF8, "application/json"
            )
        };

        // Add necessary headers including Shopify access token
        request.Headers.Add("X-Shopify-Access-Token", _credentials.AccessToken);

        var response = await client.SendAsync(request);
        var responseBody = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonConvert.DeserializeObject<JObject>(responseBody);
        var stagedTarget = jsonResponse?["data"]?["stagedUploadsCreate"]?["stagedTargets"]?.First;

        if (stagedTarget == null) return null;

        var uploadUrl = stagedTarget["url"]?.ToString();
        var resourceUrl = stagedTarget["resourceUrl"]?.ToString();

        // Deserialize parameters as a List<Parameter>
        var parameters = stagedTarget["parameters"]?.ToObject<List<Parameter>>();

        if (uploadUrl == null || parameters == null) return null;

// Step 2: Upload the Image Data
        using (var multipartFormData = new MultipartFormDataContent())
        {
            // Add all the parameters required for upload
            foreach (var param in parameters)
            {
                multipartFormData.Add(new StringContent(param.Value), param.Name);
            }

            var imageContent = new ByteArrayContent(imageData);
            imageContent.Headers.Add("Content-Type", "image/jpeg");
            multipartFormData.Add(imageContent, "file");

            var uploadResponse = await client.PostAsync(uploadUrl, multipartFormData);

            if (!uploadResponse.IsSuccessStatusCode)
            {
                // Log the error response from Shopify (optional)
                var errorResponse = await uploadResponse.Content.ReadAsStringAsync();
                Console.WriteLine("Error uploading image: " + errorResponse);
                return null;
            }
    
            // If the upload is successful, log the response body to check for resource URL
            var uploadResponseBody = await uploadResponse.Content.ReadAsStringAsync();
            Console.WriteLine("Upload Response Body: " + uploadResponseBody);

            // Parse the XML response to extract the location URL
            var responseXml = XDocument.Parse(uploadResponseBody);
            var locationUrl = responseXml.Root.Element("Location")?.Value;

            if (!string.IsNullOrEmpty(locationUrl))
            {
                locationURL = locationUrl;
            }
            else
            {
                Console.WriteLine("Error: Resource URL not found in the response.");
                return null;
            }
        }

        // Step 3: Register the uploaded image in Shopify by creating a file resource
                var createFileQuery = $@"
        mutation createFile {{
          fileCreate(files: [
            {{
              originalSource: ""{resourceUrl}""
            }}
          ]) {{
            files {{
              preview {{
                image {{
                  originalSrc
                }}
              }}
            }}
            userErrors {{
              field
              message
            }}
          }}
        }}";

        var createFileRequest = new HttpRequestMessage(HttpMethod.Post, graphqlUrl)
        {
            Content = new StringContent(
                JsonConvert.SerializeObject(new { query = createFileQuery }),
                Encoding.UTF8, "application/json"
            )
        };

        // Add the access token header to the request
        createFileRequest.Headers.Add("X-Shopify-Access-Token", _credentials.AccessToken);

        await client.SendAsync(createFileRequest);
        
        if (!string.IsNullOrEmpty(locationURL))
        {
            Console.WriteLine("Image URL from Shopify: " + locationURL);
            return locationURL;
        }
        return null;
    }
}