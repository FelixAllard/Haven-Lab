using Shopify_Api;
using Shopify_Api.Exceptions;
using ShopifySharp;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

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
        public void FormatPostProduct_MissingWeightInVariant_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Variants.FirstOrDefault().Weight = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Weight is required!"));
        }

        [Test]
        public void FormatPostProduct_InvalidWeightUnitInVariant_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Variants.FirstOrDefault().WeightUnit = "grams"; // Invalid unit

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Weight Unit is required!"));
        }

        [Test]
        public void FormatPostProduct_MissingTaxableInVariant_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Variants.FirstOrDefault().Taxable = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Taxable is required! True or false"));
        }

        [Test]
        public void FormatPostProduct_MissingRequiresShippingInVariant_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Variants.FirstOrDefault().RequiresShipping = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("RequiresShipping is required!"));
        }

        [Test]
        public void FormatPostProduct_MissingOptionName_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Options.FirstOrDefault().Name = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Name is required [OPTIONS]"));
        }

        [Test]
        public void FormatPostProduct_MissingVariantTitle_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Variants.FirstOrDefault().Title = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Title is required"));
        }

        [Test]
        public void FormatPostProduct_ValidProduct_ReturnsFormattedProduct()
        {
            var product = CreateValidProduct();
            var result = _productValidator.FormatPostProduct(product);

            Assert.That(result.Title, Is.EqualTo(product.Title));
            Assert.That(result.BodyHtml, Is.EqualTo(product.BodyHtml));
            Assert.That(result.Vendor, Is.EqualTo(product.Vendor));
            Assert.That(result.Handle, Is.EqualTo(product.Handle));
            Assert.That(result.PublishedScope, Is.EqualTo("global"));
            Assert.That(result.CreatedAt.Value.Date, Is.EqualTo(DateTime.Now.Date));
            Assert.That(result.UpdatedAt.Value.Date, Is.EqualTo(DateTime.Now.Date));
            Assert.That(result.Status, Is.EqualTo(product.Status));
        }
        [Test]
        public void FormatPostProduct_MissingTitle_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Title = null; // Set Title to null

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Title is required"));
        }

        [Test]
        public void FormatPostProduct_WhitespaceTitle_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Title = "   "; // Set Title to whitespace

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Title is required"));
        }
        [Test]
        public void FormatPostProduct_MissingBodyHtml_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.BodyHtml = null; // Set BodyHtml to null

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Description is required"));
        }

        [Test]
        public void FormatPostProduct_WhitespaceBodyHtml_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.BodyHtml = "   "; // Set BodyHtml to whitespace

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Description is required"));
        }
        [Test]
        public void FormatPostProduct_MissingVendor_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Vendor = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Vendor is required"));
        }
        [Test]
        public void FormatPostProduct_MissingHandle_SetsDefaultHandle()
        {
            var product = CreateValidProduct();
            product.Handle = null;

            var result = _productValidator.FormatPostProduct(product);

            Assert.That(result.Handle, Is.EqualTo("new-product"));
        }

        [Test]
        public void FormatPostProduct_MissingStatus_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Status = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Status is required. must be [active] or [draft]"));
        }

        [Test]
        public void FormatPostProduct_InvalidStatus_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Status = "invalid-status";

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Status must be [draft] or [active] or [draft]"));
        }

        [Test]
        public void FormatPostProduct_MissingPriceInVariant_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Variants.FirstOrDefault().Price = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("Price is required"));
        }


        [Test]
        public void FormatPostProduct_MissingInventoryQuantityInVariant_ThrowsInputException()
        {
            var product = CreateValidProduct();
            product.Variants.FirstOrDefault().InventoryQuantity = null;

            var ex = Assert.Throws<InputException>(() => _productValidator.FormatPostProduct(product));
            Assert.That(ex.Message, Is.EqualTo("InventoryQuantity is required!"));
        }



        [Test]
        public void FormatPostProduct_ImagesSet_IdShouldBeNullified()
        {
            var product = CreateValidProduct();
            product.Images = new List<ProductImage>
            {
                new ProductImage { Id = 1, Src = "image1.jpg" },
                new ProductImage { Id = 2, Src = "image2.jpg" }
            };

            var result = _productValidator.FormatPostProduct(product);

            foreach (var image in result.Images)
            {
                Assert.That(image.Id, Is.Null);
            }
        }
        [Test]
        public void FormatPostProduct_DraftStatus_SetsPublishedAtToCurrentDate()
        {
            // Arrange
            var product = CreateValidProduct();
            product.Status = "draft";

            // Act
            var result = _productValidator.FormatPostProduct(product);
            
            // Assert
            Assert.IsNotNull(result.PublishedAt, "PublishedAt should not be null");
        }



    }
}
