import React, { useEffect, useState } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';

const CartPage = () => {
    const [cart, setCart] = useState({});

    useEffect(() => {
        fetchCart();
    }, []);

    const fetchCart = async () => {
        try {
            const response = await axios.get('http://localhost:5158/gateway/api/Cart', { withCredentials: true });
            if (response.status === 200) {
                console.log('Cart fetched successfully:', response.data);
                setCart(response.data);
                return response.data;
            } else {
                console.error('Failed to fetch cart.');
            }
        } catch (err) {
            console.error('Error fetching cart:', err);
        }
    };

    const removeByOne = async (variantId) => {
        try {
            const response = await axios.post(
                `http://localhost:5158/gateway/api/Cart/removebyone/${variantId}`,
                null,
                { withCredentials: true }
            );
            if (response.status === 200) {
                console.log('Product quantity reduced by one:', response.data);
                setCart(response.data);
            } else {
                console.error('Failed to reduce product quantity.');
            }
        } catch (err) {
            console.error('Error reducing quantity by one:', err);
        }
    };

    const updateQuantity = async (variantId, newQuantity) => {
        if (newQuantity < 0) {
            console.warn('Invalid quantity. Skipping update.');
            return;
        }
        if (newQuantity === 0) {
            await removeFromCart(variantId);
        } else if (newQuantity < cart[variantId]) {
            await removeByOne(variantId);
        } else {
            try {
                const response = await axios.post(
                    `http://localhost:5158/gateway/api/Cart/addbyone/${variantId}`,
                    null,
                    { withCredentials: true }
                );
                if (response.status === 200) {
                    console.log('Product quantity increased:', response.data);
                    setCart(response.data);
                } else {
                    console.error('Failed to increase product quantity.');
                }
            } catch (err) {
                console.error('Error increasing quantity:', err);
            }
        }
    };

    const removeFromCart = async (variantId) => {
        try {
            const response = await axios.post(
                `http://localhost:5158/gateway/api/Cart/remove/${variantId}`,
                null,
                { withCredentials: true }
            );
            if (response.status === 200) {
                console.log('Product removed from cart:', response.data);
                setCart(response.data);
            } else {
                console.error('Failed to remove product from cart.');
            }
        } catch (err) {
            console.error('Error removing product:', err);
        }
    };

    const handleCreateDraftOrder = async () => {
        try {
            // Fetch the cart data
            const cartResponse = await fetchCart();
            console.log('Cart Response:', cartResponse);

            // Check if the cart is not empty
            if (!cartResponse || Object.keys(cartResponse).length === 0) {
                console.log('Cart is empty, no draft order to create.');
                return;
            }

            // Map the cart items to the draft order format with individual objects
            const lineItems = Object.keys(cartResponse).map(variantId => ({
                variant_id: parseInt(variantId), // Ensure variant_id is a number
                quantity: cartResponse[variantId]
            }));

            const draftOrderData = {
                line_items: lineItems
            };

            console.log('Draft Order Data:', JSON.stringify(draftOrderData)); // Print the JSON to console for debugging

            // Send the request to create a draft order
            const response = await axios.post(
                'http://localhost:5158/gateway/api/ProxyDraftOrder',
                draftOrderData,
                { withCredentials: true }
            );

            if (response.status === 200) {
                console.log('Draft order created successfully:', response.data);
                const invoiceUrl = response.data; // Adjust if necessary
                if (invoiceUrl) {
                    window.location.href = invoiceUrl;
                } else {
                    console.error('No invoice URL received');
                }
            } else {
                console.error('Failed to create draft order.');
            }
        } catch (err) {
            console.error('Error creating draft order:', err);
        }
    };



    return (
        <div className="cart-page mt-6">
            <h1>Your Cart</h1>
            <ul>
                {Object.keys(cart).map((productId) => (
                    <li key={productId} className="cart-item">
                        <span>Product ID: {productId}</span>
                        <span>Quantity: </span>
                        <button onClick={() => updateQuantity(productId, cart[productId] - 1)}>-</button>
                        <span>{cart[productId]}</span>
                        <button onClick={() => updateQuantity(productId, cart[productId] + 1)}>+</button>
                        <button onClick={() => removeFromCart(productId)}>Remove</button>
                    </li>
                ))}
            </ul>

            <button onClick={handleCreateDraftOrder} className="btn btn-primary">Create Draft Order</button>
        </div>
    );
};

export default CartPage;
