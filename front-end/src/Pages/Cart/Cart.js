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
            const response = await axios.get('http://localhost:5158/gateway/api/Cart', null, { withCredentials: true });
            if (response.status === 200) {
                console.log('200');
                console.log(response.data);
                setCart(response.data);
            } else {
                console.log('failed');
                console.error('Failed to fetch cart.');
            }
        } catch (err) {
            console.log('failed');
            console.error('Error fetching cart:', err);
        }
    };

    const updateQuantity = async (productId, newQuantity) => {
        if (newQuantity <= 0) {
            await removeFromCart(productId);
            return;
        }

        try {
            const response = await axios.post(`http://localhost:5158/gateway/api/Cart/add/${productId}`, null, { withCredentials: true });
            if (response.status === 200) {
                setCart(response.data);
            } else {
                console.error('Failed to update product quantity.');
            }
        } catch (err) {
            console.error('Error updating quantity:', err);
        }
    };

    const removeFromCart = async (productId) => {
        try {
            const response = await axios.post(`http://localhost:5158/gateway/api/Cart/remove/${productId}`, null, { withCredentials: true });
            if (response.status === 200) {
                setCart(response.data);
            } else {
                console.error('Failed to remove product from cart.');
            }
        } catch (err) {
            console.error('Error removing product:', err);
        }
    };

    // Hardcoded Draft Order data
    const handleCreateDraftOrder = async () => {
        const draftOrderData = {
            line_items: [
                {
                    variant_id: 43173936758829,
                    quantity: 2
                }
            ]
        };

        try {
            const response = await axios.post('http://localhost:5158/gateway/api/ProxyDraftOrder', draftOrderData, { withCredentials: true });
            if (response.status === 200) {
                console.log('Draft order created successfully:', response.data);

                // Assuming the response contains an "InvoiceUrl" field with the URL to redirect to
                const invoiceUrl = response.data;  // Modify this if necessary to extract the URL correctly from the response data
                if (invoiceUrl) {
                    window.location.href = invoiceUrl;  // Redirect the user to the invoice URL
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
        <div className="cart-page">
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

            <h2>Recommended Products</h2>
            <ul>
                <li className="cart-item">
                    <span>Product Name: Dummy Product 1</span>
                    <span>Price: $29.99</span>
                </li>
                <li className="cart-item">
                    <span>Product Name: Dummy Product 2</span>
                    <span>Price: $19.99</span>
                </li>
                <li className="cart-item">
                    <span>Product Name: Dummy Product 3</span>
                    <span>Price: $15.00</span>
                </li>
                <li className="cart-item">
                    <span>Product Name: Dummy Product 4</span>
                    <span>Price: $25.50</span>
                </li>
                <li className="cart-item">
                    <span>Product Name: Dummy Product 5</span>
                    <span>Price: $30.00</span>
                </li>
            </ul>

            <button onClick={handleCreateDraftOrder} className="btn btn-primary">Create Draft Order</button>
        </div>
    );
};

export default CartPage;
