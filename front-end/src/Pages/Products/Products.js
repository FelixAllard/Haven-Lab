import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'motion/react';

// Bootstrap CSS for card styling
import 'bootstrap/dist/css/bootstrap.min.css';

const ProductPage = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Fetch products on component mount
    useEffect(() => {
        const fetchProducts = async () => {
            try {
                const response = await axios.get('http://localhost:5158/gateway/api/ProxyProduct');
                setProducts(response.data.items);
            } catch (err) {
                setError(err.message);
            } finally {
                setLoading(false);
            }
        };

        fetchProducts();
    }, []);

    // Render loading or error states
    if (loading) {
        return <div className="text-center">Loading...</div>;
    }

    if (error) {
        return <div className="text-center text-danger">Error: {error}</div>;
    }

    return (

        <div className="container mt-4">
            <br/><br/>
            <h1>Our Products</h1>
            <br/>
            <div className="row">
                {products.map((product, index) => (
                    <motion.div
                        className="col-md-4 mb-4"
                        key={product.id}
                        initial={{opacity: 0, y: 50}}
                        animate={{opacity: 1, y: 0}}
                        transition={{duration: 0.5, delay: index * 0.1}}
                    >
                        <div className="card shadow-sm border-light">
                            <img
                                src={product.images[0]?.src || 'https://via.placeholder.com/150'}
                                className="card-img-top"
                                alt={product.title}
                                style={{height: '200px', objectFit: 'cover'}}
                            />
                            <div className="card-body">
                                <h5 className="card-title">{product.title}</h5>
                                <div
                                    className="card-text"
                                    dangerouslySetInnerHTML={{__html: product.bodyHtml}}
                                />
                                <p><strong>Quantity:</strong> {product.variants[0]?.inventoryQuantity}</p>
                                <p><strong>Price:</strong> ${product.variants[0]?.price}</p>
                            </div>
                        </div>
                    </motion.div>
                ))}
            </div>
        </div>
    );
};

export default ProductPage;
