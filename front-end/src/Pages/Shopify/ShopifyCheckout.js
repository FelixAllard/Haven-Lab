import React, { useState, useEffect } from 'react';
import client from 'shopify-buy';

const Checkout = () => {
    const [products, setProducts] = useState([]);
    const [checkoutUrl, setCheckoutUrl] = useState(null);

    useEffect(() => {
        // Fetch products when the component mounts
        client.product.fetchAll().then((fetchedProducts) => {
            setProducts(fetchedProducts);
        });
    }, []);

    const handleCheckout = async (productId) => {
        const checkout = await client.checkout.create();
        const lineItemsToAdd = [{ variantId: productId, quantity: 1 }];

        const updatedCheckout = await client.checkout.addLineItems(
            checkout.id,
            lineItemsToAdd
        );

        setCheckoutUrl(updatedCheckout.webUrl);
    };

    return (
        <div>
            <h1>Products</h1>
            {products.map((product) => (
                <div key={product.id}>
                    <h2>{product.title}</h2>
                    <button onClick={() => handleCheckout(product.variants[0].id)}>
                        Buy Now
                    </button>
                </div>
            ))}

            {checkoutUrl && (
                <a href={checkoutUrl} target="_blank" rel="noopener noreferrer">
                    Go to Checkout
                </a>
            )}
        </div>
    );
};

export default Checkout;
