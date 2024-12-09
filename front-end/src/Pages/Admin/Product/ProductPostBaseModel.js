const productPostBaseModel = {
    title: "New Product",
    body_html: "<p>This is a new product description</p>",
    created_at: new Date().toISOString(), // You can dynamically set the date or leave it static
    updated_at: new Date().toISOString(),
    published_at: new Date().toISOString(),
    vendor: "New Vendor",
    product_type: "",
    handle: "new-product",
    template_suffix: null,
    published_scope: "global",
    tags: "",
    status: "active",
    variants: [
        {
            product_id: null,
            title: "Default Title",
            sku: null,
            position: 1,
            grams: 0,
            inventory_policy: "deny",
            fulfillment_service: "manual",
            inventory_item_id: null,
            inventory_management: null,
            price: 19.99,
            compare_at_price: null,
            option1: "Default Title",
            option2: null,
            option3: null,
            created_at: new Date().toISOString(),
            updated_at: new Date().toISOString(),
            taxable: true,
            tax_code: null,
            requires_shipping: true,
            barcode: null,
            inventory_quantity: 5,
            image_id: null,
            weight: 0,
            weight_unit: "kg",
            metafields: null,
            presentment_prices: null,
            id: null,
            admin_graphql_api_id: null
        }
    ],
    options: [
        {
            product_id: null,
            name: "Title",
            position: 1,
            values: [
                "Default Title"
            ],
            id: null,
            admin_graphql_api_id: null
        }
    ],
    images: [],
    metafields: null,
    variant_gids: null,
    id: null,
    admin_graphql_api_id: null
};