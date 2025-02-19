import React, { useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import './Cart.css';
import httpClient from '../../AXIOS/AXIOS';
import '../../Languages/i18n.js';
import Cookies from 'js-cookie';
import { useTranslation } from 'react-i18next';
import HoverScaleWrapper from '../../Shared/HoverScaleWrapper';

const CartPage = () => {
  const [cart, setCart] = useState([]);
  const { t } = useTranslation('cart');

  useEffect(() => {
    const savedCart = JSON.parse(Cookies.get('Cart') || '[]');
    setCart(savedCart);
  }, []);

  const removeFromCart = (productId) => {
    const updatedCart = cart.filter((item) => item.productId !== productId);
    setCart(updatedCart);
    Cookies.set('Cart', JSON.stringify(updatedCart), { expires: 7 });
  };

  const removeByOne = (productId) => {
    const updatedCart = cart
      .map((item) => {
        if (item.productId === productId) {
          return {
            ...item,
            quantity: item.quantity - 1,
          };
        }
        return item;
      })
      .filter((item) => item.quantity > 0);

    setCart(updatedCart);
    Cookies.set('Cart', JSON.stringify(updatedCart), { expires: 7 });
  };

  const addByOne = async (productId) => {
    try {
      const response = await httpClient.get(
        `/gateway/api/ProxyProduct/${productId}`,
      );

      if (response.status === 200) {
        const productData = response.data;
        const availableQuantity =
          productData.variants[0]?.inventory_quantity || 0;

        // Find the current cart item
        const cartItem = cart.find((item) => item.productId === productId);

        if (!cartItem) {
          console.error('Item not found in the cart');
          return;
        }

        // Check if adding one more exceeds the available quantity
        if (cartItem.quantity + 1 > availableQuantity) {
          alert('Sorry, no more stock available for this product.');
          return;
        }

        // If stock is available, increase the quantity by one
        const updatedCart = cart.map((item) => {
          if (item.productId === productId) {
            return {
              ...item,
              quantity: item.quantity + 1,
            };
          }
          return item;
        });

        // Update the state and the cookie
        setCart(updatedCart);
        Cookies.set('Cart', JSON.stringify(updatedCart), { expires: 7 });
      } else {
        console.error('Failed to fetch product details.');
      }
    } catch (error) {
      console.error('Error fetching product details:', error.message);
    }
  };

  const handleCreateDraftOrder = async () => {
    try {
      // Fetch the cart data
      const cartResponse = cart;
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

      console.log(lineItems);

      const draftOrderData = {
        line_items: lineItems,
      };

      console.log('Draft Order Data:', JSON.stringify(draftOrderData)); // Print the JSON to console for debugging

      // Send the request to create a draft order
      const response = await httpClient.post(
        `/gateway/api/ProxyDraftOrder`,
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
    <div className="container mt-7 position-relative">
      <h1 className="text-center">{t('Your Cart')}</h1>
      {cart.length === 0 ? (
        <h2 className="empty-cart-message text-center">{t('Empty Cart')}</h2>
      ) : (
        <div className="cart-container">
          <hr className="cart-divider" />
          {cart.map((item, index) => (
            <React.Fragment key={`${item.productId}-${index}`}>
              <div className="cart-item-card">
                {/* Product Image */}
                <img
                  src={
                    item.imageSrc || require('../../Shared/imageNotFound.jpg')
                  }
                  alt={item.productTitle}
                  className="cart-item-image"
                />

                {/* Product Info */}
                <div className="cart-item-details">
                  <h2>
                    {Cookies.get('language') === 'fr'
                      ? item.frenchProductTitle || item.productTitle
                      : item.productTitle}
                  </h2>
                  <button
                    className="remove-btn"
                    onClick={() => removeFromCart(item.productId)}
                  >
                    {t('Remove')}
                  </button>
                </div>

                {/* Quantity Controls */}
                <div className="cart-item-quantity">
                  <HoverScaleWrapper>
                    <button
                      className="quantity-btn-left"
                      onClick={() => removeByOne(item.variantId)}
                    >
                      {' '}
                      -
                    </button>
                  </HoverScaleWrapper>

                  <span className="quantity">{item.quantity}</span>
                  <HoverScaleWrapper>
                    <button
                      className="quantity-btn-right"
                      onClick={() => addByOne(item.variantId)}
                    >
                      {' '}
                      +
                    </button>
                  </HoverScaleWrapper>
                </div>

                {/* Price */}
                <div className="cart-item-price">
                  <span>${(item.price * item.quantity).toFixed(2)}</span>
                </div>
              </div>

              {/* Add White Divider Between Items */}
              {index < cart.length - 1 && <hr className="cart-divider"></hr>}
            </React.Fragment>
          ))}

          <hr className="cart-divider" />

          {/* Subtotal Calculation */}
          <h3 className="subtotal">
            {t('Subtotal')}
            <br></br> $
            {cart
              .reduce((total, item) => total + item.price * item.quantity, 0)
              .toFixed(2)}
          </h3>

          <button
            onClick={handleCreateDraftOrder}
            className="checkout-btn justify-content-end"
          >
            <HoverScaleWrapper>{t('Checkout')}</HoverScaleWrapper>
          </button>
        </div>
      )}
    </div>
  );
};

export default CartPage;
