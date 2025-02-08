import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import bootstrap from 'bootstrap/dist/js/bootstrap.min.js';
import { useAuth } from '../../AXIOS/AuthentificationContext';
import './ProductDetailPage.css';
import 'bootstrap/dist/css/bootstrap.min.css';
import { FaArrowLeft } from 'react-icons/fa';
import httpClient from '../../AXIOS/AXIOS';

const ProductDetailsPage = () => {
  const { productId } = useParams(); // Get product ID from the URL
  const [product, setProduct] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [toastMessage, setToastMessage] = useState(''); // Store toast message
  const navigate = useNavigate(); // Hook to programmatically navigate
  const toastTrigger = document.getElementById('liveToastBtn');
  const toastLiveExample = document.getElementById('liveToast');

  const { authToken } = useAuth();
  const isLoggedIn = !!authToken;

  if (toastTrigger) {
    const toastBootstrap =
      bootstrap.Toast.getOrCreateInstance(toastLiveExample);
    toastTrigger.addEventListener('click', () => {
      toastBootstrap.show();
    });
  }

  // Fetch product details when the component mounts
  useEffect(() => {
    const fetchProductDetails = async () => {
      try {
        const response = await httpClient.get(
          `/gateway/api/ProxyProduct/${productId}`,
        );
        setProduct(response.data);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchProductDetails();
  }, [productId]);

  const handleDelete = async () => {
    try {
      const response = await httpClient.delete(
        `/gateway/api/ProxyProduct/${productId}`,
      );
      if (response.status === 200) {
        setToastMessage('Product successfully deleted.');
        setTimeout(() => {
          navigate('/products'); // Redirect to products page after success
        }, 1000); // Wait for toast to show before redirect
      } else {
        setToastMessage('Failed to delete product.');
      }
    } catch (err) {
      setToastMessage('Error: ' + err.message);
    }
  };

  const addToCart = async () => {
    try {
      console.log('Adding product to cart...');
      const response = await httpClient.post(
        `/gateway/api/ProxyCart/add/${productId}`,
        null,
        { withCredentials: true },
      );
      if (response.status === 200) {
        const updatedCartResponse = await httpClient.get(
          `/gateway/api/ProxyCart`,
          null,
          { withCredentials: true },
        );
        if (updatedCartResponse.status === 200) {
          console.log('Updated cart:', updatedCartResponse.data);
        }
      } else {
        setToastMessage('Failed to add product to cart.');
      }
    } catch (err) {
      console.error('Error adding product to cart:', err);
      setToastMessage('Error: ' + err.message);
    }
  };

  // Render loading or error states
  if (loading) {
    return <div className="text-center">Loading product details...</div>;
  }

  if (error) {
    return <div className="text-center text-danger">Error: {error}</div>;
  }

  return (
    <div className="container mt-6 position-relative">
      {/* Back arrow link */}
      <Link
        to="/products"
        className="btn btn-link text-dark position-absolute"
        style={{ top: '20px', left: '20px', zIndex: 10 }}
      >
        <FaArrowLeft size={30} />
      </Link>

      <motion.div
        initial={{ opacity: 0 }}
        animate={{ opacity: 1 }}
        transition={{ duration: 1 }}
      >
        <h1 className="display-4 text-center">{product.title}</h1>
      </motion.div>

      <motion.div
        className="row justify-content-center mt-4"
        initial={{ opacity: 0, scale: 0.5 }}
        animate={{ opacity: 1, scale: 1 }}
        transition={{ duration: 1 }}
      >
        {/* Left Column: Product Image */}
        <div className="col-md-4">
          <motion.img
            src={
              product.images[0]?.src ||
              require('../../Shared/imageNotFound.jpg')
            }
            className="card-img-top mb-4"
            alt={product.title}
            style={{ height: '400px', objectFit: 'cover' }}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{ duration: 1 }}
          />
        </div>

        {/* Right Column: Product Details */}
        <div className="col-md-6">
          <div className="card shadow-lg p-4">
            {/* Toast Message */}
            <div className="toast-container position-fixed bottom-0 end-0 p-3">
              <div
                id="liveToast"
                className="toast"
                role="alert"
                aria-live="assertive"
                aria-atomic="true"
              >
                <div className="toast-header">
                  <strong className="me-auto">Shopify</strong>
                  <button
                    type="button"
                    className="btn-close"
                    data-bs-dismiss="toast"
                    aria-label="Close"
                  ></button>
                </div>
                <div className="toast-body">{toastMessage}</div>
              </div>
            </div>

            {/* Product Description */}
            <motion.p
              className="lead"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1 }}
            >
              <strong>Description:</strong>
            </motion.p>
            <div
              className="card-text mt-2"
              dangerouslySetInnerHTML={{ __html: product.body_html }}
            />

            {/* Vendor */}
            <motion.p
              className="mt-3"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1, delay: 0.3 }}
            >
              <strong>Vendor:</strong> {product.vendor}
            </motion.p>

            {/* Product Weight */}
            <motion.p
              className="mt-3"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1, delay: 0.6 }}
            >
              <strong>Weight:</strong> {product.variants[0]?.weight}{' '}
              {product.variants[0]?.weight_unit}
            </motion.p>

            {/* Price */}
            <motion.p
              className="mt-3"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1, delay: 0.9 }}
            >
              <strong>Price:</strong> ${product.variants[0]?.price.toFixed(2)}
            </motion.p>

            {/* Quantity Available */}
            <motion.p
              className="mt-3"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1, delay: 1.2 }}
            >
              <strong>Quantity Available:</strong>{' '}
              {product.variants[0]?.inventory_quantity}
            </motion.p>

            {/* Shipping Requirement */}
            <motion.p
              className="mt-3"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1, delay: 1.5 }}
            >
              <strong>Requires Shipping:</strong>{' '}
              {product.variants[0]?.requires_shipping ? 'Yes' : 'No'}
            </motion.p>

            {/* Created At */}
            <motion.p
              className="mt-3"
              initial={{ opacity: 0 }}
              animate={{ opacity: 1 }}
              transition={{ duration: 1, delay: 1.8 }}
            >
              <strong>Created At:</strong>{' '}
              {new Date(product.created_at).toLocaleDateString()}
            </motion.p>

            {/* Add to Cart Button (Only for users) */}
            {!isLoggedIn && (
              <motion.button
                className="btn btn-success mb-3"
                whileHover={{ scale: 1.1 }}
                transition={{ duration: 0.2 }}
                onClick={addToCart}
              >
                Add to Cart
              </motion.button>
            )}

            {/* Edit product Button - Owner */}
            {isLoggedIn && (
              <motion.button
                className="btn btn-secondary mt-2"
                whileHover={{ scale: 1.1 }}
                transition={{ duration: 0.2 }}
              >
                <Link
                  to={`/admin/product/update/${product.id}`}
                  style={{ color: 'white', textDecoration: 'none' }}
                >
                  Update Product
                </Link>
              </motion.button>
            )}

            {/* Delete Button - Owner */}
            {isLoggedIn && (
              <motion.button
                className="btn btn-danger mb-3 mt-3"
                id="liveToastBtn"
                whileHover={{ scale: 1.1 }}
                transition={{ duration: 0.2 }}
                onClick={handleDelete}
              >
                Delete Product
              </motion.button>
            )}
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default ProductDetailsPage;
