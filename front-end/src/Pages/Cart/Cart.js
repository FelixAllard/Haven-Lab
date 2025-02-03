import React, { useEffect, useState } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;
const CartPage = () => {
  const [cart, setCart] = useState([]);

  useEffect(() => {
    fetchCart();
  }, []);

  const fetchCart = async () => {
    try {
      const response = await axios.get(`${environment}/gateway/api/ProxyCart`, {
        withCredentials: true,
      });
      if (response.status === 200) {
        console.log('Cart fetched successfully:', response.data);

        // Ensure cart is an array
        if (Array.isArray(response.data)) {
          setCart(response.data);
        } else {
          setCart([]);
        }

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
          `${environment}/gateway/api/ProxyCart/removebyone/${variantId}`,
          null,
          { withCredentials: true },
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

  const addByOne = async (variantId) => {
    try {
      const response = await axios.post(
          `${environment}/gateway/api/ProxyCart/addbyone/${variantId}`,
          null,
          { withCredentials: true },
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
  };

  const removeFromCart = async (variantId) => {
    try {
      const response = await axios.post(
          `${environment}/gateway/api/ProxyCart/remove/${variantId}`,
          null,
          { withCredentials: true },
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

      // Ensure cart is an array
      if (!Array.isArray(cartResponse) || cartResponse.length === 0) {
        console.log('Cart is empty, no draft order to create.');
        return;
      }

      // Map cart items to only include variant_id and quantity
      const lineItems = cartResponse.map((item) => ({
        variant_id: item.variantId, // Ensure we're using the correct property
        quantity: item.quantity,
      }));

      const draftOrderData = {
        line_items: lineItems,
      };

      console.log('Draft Order Data:', JSON.stringify(draftOrderData)); // Print the JSON to console for debugging

      // Send the request to create a draft order
      const response = await axios.post(
          `${environment}/gateway/api/ProxyDraftOrder`,
          draftOrderData,
          { withCredentials: true },
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
          {cart.map((item) => (
              <li key={item.variantId} className="cart-item">
                <span>{item.productTitle}</span>
                <span>Price: ${item.price.toFixed(2)}</span>
                <span>Quantity: </span>
                <button onClick={() => removeByOne(item.variantId)}> - </button>
                <span>{item.quantity}</span>
                <button onClick={() => addByOne(item.variantId)}> + </button>
                <button onClick={() => removeFromCart(item.variantId)}>
                  Remove
                </button>
              </li>
          ))}
        </ul>

        {/* Subtotal Calculation */}
        <h3>
          Subtotal: $
          {cart
              .reduce((total, item) => total + item.price * item.quantity, 0)
              .toFixed(2)}
        </h3>

        <button onClick={handleCreateDraftOrder} className="btn btn-primary">
          Create Draft Order
        </button>
      </div>
  );
};

export default CartPage;