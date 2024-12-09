using Shopify_Api.Exceptions;
using ShopifySharp;

namespace Shopify_Api;

public class ProductValidator
{
    
    public Product FormatPostProduct(Product product, bool takeCurrentDate = true)
    {
        Product p = product;
        if(String.IsNullOrWhiteSpace(p.Title))
            throw new InputException("Title is required");
        if(String.IsNullOrWhiteSpace(p.BodyHtml))
            throw new InputException("Description is required");
        
        if (takeCurrentDate)
        {
            DateTime currentDate = DateTime.Now;
            p.CreatedAt = currentDate;
            p.UpdatedAt = currentDate;

            //Check if the product is currently published
            if (product.Status == "draft" )
            {
                p.PublishedAt = currentDate;
            }
        }

        p.ProductType = "";
        if(String.IsNullOrWhiteSpace(p.Vendor))
            throw new InputException("Vendor is required");
        p.TemplateSuffix = null;
        p.PublishedScope = "global";
        p.Tags = "";
        if(String.IsNullOrWhiteSpace(p.Handle))
            p.Handle = "new-product";
        //Publish scope????? I didn't find it
        if (String.IsNullOrWhiteSpace(p.Status))
            throw new InputException("Status is required. must be [active] or [draft]");
        if(p.Status != "draft" && p.Status != "active" && p.Status != "archived")
            throw new InputException("Status must be [draft] or [active] or [draft]");
        
        
        foreach (var variant in p.Variants)
        {
            variant.ProductId = null;
            if(String.IsNullOrWhiteSpace(variant.Title))
                throw new InputException("Title is required");
            variant.SKU = null;
            variant.Position = 1;
            variant.Grams = 0;
            variant.InventoryPolicy = "deny";
            variant.FulfillmentService = "manual";
            variant.InventoryItemId = null;
            variant.InventoryManagement = null;
            if(variant.Price == null)
                throw new InputException("Price is required");
            variant.CompareAtPrice = null;
            variant.Option1 = "Default Title";
            variant.Option2 = null;
            variant.Option3 = null;
            
            DateTime currentDate = DateTime.Now;
            variant.CreatedAt = currentDate;
            variant.UpdatedAt = currentDate;
            
            if (variant.Taxable == null)
                throw new InputException("Taxable is required! True or false");
            variant.TaxCode = null;
            if (variant.RequiresShipping == null)
                throw new InputException("RequiresShipping is required!");
            variant.Barcode = null;
            if (variant.InventoryQuantity==null)
                throw new InputException("InventoryQuantity is required!");
            variant.ImageId = null;
            if(variant.Weight==null)
                throw new InputException("Weight is required!");
            if(!(variant.WeightUnit == "lb" || variant.WeightUnit == "kg"))
                throw new InputException("Weight Unit is required!");
            variant.Metafields = null;
            variant.PresentmentPrices = null;
            variant.Id = null;
            variant.AdminGraphQLAPIId = null;
        }

        if (p.Options != null)
        {
            foreach (var option in p.Options)
            {
                option.ProductId = null;
                if(String.IsNullOrWhiteSpace(option.Name))
                    throw new InputException("Name is required [OPTIONS] ");
                option.Position = 1;
                option.Values = new List<string> { "Default Title" };
                option.Id = null;
                option.AdminGraphQLAPIId = null;
            }
        }

        if (p.Images != null)
        {
            foreach (var image in p.Images)
            {
                image.Id = null;
            }
        }

        return p;
    }
}