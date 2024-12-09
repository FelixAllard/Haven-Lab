using Shopify_Api;
using Shopify_Api.Exceptions;
using ShopifySharp;

namespace TestingProject.Shopify_Api.SRC
{
    [TestFixture]
    public class ProductValidatorTest
    {
        private ProductValidator _productValidator;

        [SetUp]
        public void SetUp()
        {
            _productValidator = new ProductValidator();
        }

        // Helper method to create a basic product with required fields.
        private Product CreateValidProduct()
        {
            return new Product
            {
                Title = "Test Product",
                BodyHtml = "<p>Test Description</p>",
                Vendor = "Test Vendor",
                Handle = "test-product",
                Status = "active",
                Variants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Title = "Test Variant",
                        Price = 10.00m,
                        Taxable = true,
                        RequiresShipping = true,
                        Weight = 1.0m,
                        WeightUnit = "kg",
                        InventoryQuantity = 10
                    }
                },
                Options = new List<ProductOption>
                {
                    new ProductOption
                    {
                        Name = "Size",
                        Values = new List<string> { "M", "L" }
                    }
                },
                Images = new List<ProductImage>()
            };
        }
        [Test]
        [TestCase (null)]
        [TestCase ("")]
        public void FormatPostProduct_ValidProduct_ReturnsFormattedProduct(string? testcase)
        {
            // Arrange
            var product = CreateValidProduct();
            product.Handle = testcase;
            // Act
            var result = _productValidator.FormatPostProduct(product);

            // Assert
            Assert.That(result.Title, Is.EqualTo(product.Title));
            Assert.That(result.BodyHtml, Is.EqualTo(product.BodyHtml));
            Assert.That(result.Vendor, Is.EqualTo(product.Vendor));
            Assert.That(result.Handle, Is.EqualTo("new-product")); // Default handle if not provided
            Assert.That(result.PublishedScope, Is.EqualTo("global"));
    
            // Check if CreatedAt and UpdatedAt match the current date (ignoring the time)
            Assert.That(result.CreatedAt.Value.Date, Is.EqualTo(DateTime.Now.Date));  // Accessing the Date property of DateTimeOffset
            Assert.That(result.UpdatedAt.Value.Date, Is.EqualTo(DateTime.Now.Date)); // Accessing the Date property of DateTimeOffset

            Assert.That(result.Status, Is.EqualTo("active"));
        }


        [Test]
        public void FormatPostProduct_MissingTitle_ThrowsInputException()
        {
            // Arrange
            var product = CreateValidProduct();
            product.Title = null;

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Title is required"));
        }
        [Test]
        public void FormatPostProduct_MissingDescription_ThrowsInputException()
        {
            // Arrange
            var product = CreateValidProduct();
            product.BodyHtml = null;

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Description is required"));
        }
        [Test]
        public void FormatPostProduct_MissingVendor_ThrowsInputException()
        {
            // Arrange
            var product = CreateValidProduct();
            product.Vendor = null;

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Vendor is required"));
        }
        [Test]
        public void FormatPostProduct_MissingStatus_ThrowsInputException()
        {
            // Arrange
            var product = CreateValidProduct();
            product.Status = null;

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Status is required. must be [active] or [draft]"));
        }

        [Test]
        public void FormatPostProduct_InvalidStatus_ThrowsInputException()
        {
            // Arrange
            var product = CreateValidProduct();
            product.Status = "invalid-status"; // Invalid status value

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
    
            // Ensure the correct exception message is thrown for invalid status
            Assert.That(ex.Message, Is.EqualTo("Status must be [draft] or [active] or [draft]"));
        }


        [Test]
        public void FormatPostProduct_MissingPriceInVariant_ThrowsInputException()
        {
            // Arrange
            var product = CreateValidProduct();
            var variant = product.Variants.FirstOrDefault();
            if (variant != null)
            {
                variant.Price = null;
            }

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Price is required"));
        }


        [Test]
        public void FormatPostProduct_MissingInventoryQuantityInVariant_ThrowsInputException()
        {
            // Arrange
            var product = CreateValidProduct();
            var variant = product.Variants.FirstOrDefault();
            if (variant != null)
            {
                variant.InventoryQuantity = null;
            }

            // Act & Assert
            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("InventoryQuantity is required!"));
        }
    }
}
