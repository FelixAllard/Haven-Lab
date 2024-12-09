import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams, Link } from 'react-router-dom'; // Hook to get URL params
import { motion } from 'motion/react'; // Motion for animation effects

// Bootstrap CSS for card styling
import 'bootstrap/dist/css/bootstrap.min.css';
import { FaArrowLeft } from 'react-icons/fa'; // FontAwesome icon for the back arrow

const ProductDetailsPage = () => {
    const { productId } = useParams(); // Get product ID from the URL
    const [product, setProduct] = useState(null);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Fetch product details when the component mounts
    useEffect(() => {
        const fetchProductDetails = async () => {
            try {
                const response = await axios.get(`http://localhost:5158/gateway/api/ProxyProduct/${productId}`);
                setProduct(response.data);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchProductDetails();
    }, [productId]);

    // Render loading or error states
    if (loading) {
        return <div className="text-center">Loading product details...</div>;
    }

    if (error) {
        return <div className="text-center text-danger">Error: {error}</div>;
    }

    return (
        <div className="container mt-5 position-relative">
            {/* Back arrow link */}
            <Link to="/products" className="btn btn-link text-dark position-absolute" style={{ top: '20px', left: '20px', zIndex: 10 }}>
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
                        src={product.images[0]?.src || 'https://via.placeholder.com/150'}
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
                    <div className="card shadow-lg border-light p-4">
                        {/* Product Description */}
                        <motion.p
                            className="lead"
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            transition={{ duration: 1 }}
                        >
                            <strong>Description:</strong>
                            <div
                                className="card-text mt-2"
                                dangerouslySetInnerHTML={{ __html: product.body_html }}
                            />
                        </motion.p>

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
                            <strong>Weight:</strong> {product.variants[0]?.weight} {product.variants[0]?.weight_unit}
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
                            <strong>Quantity Available:</strong> {product.variants[0]?.inventory_quantity}
                        </motion.p>

                        {/* Shipping Requirement */}
                        <motion.p
                            className="mt-3"
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            transition={{ duration: 1, delay: 1.5 }}
                        >
                            <strong>Requires Shipping:</strong> {product.variants[0]?.requires_shipping ? 'Yes' : 'No'}
                        </motion.p>

                        {/* Created At */}
                        <motion.p
                            className="mt-3"
                            initial={{ opacity: 0 }}
                            animate={{ opacity: 1 }}
                            transition={{ duration: 1, delay: 1.8 }}
                        >
                            <strong>Created At:</strong> {new Date(product.created_at).toLocaleDateString()}
                        </motion.p>
                        <motion.button
                                    className="btn btn-secondary"
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
                    </div>
                </div>
            </motion.div>
        </div>
    );
};

export default ProductDetailsPage;
