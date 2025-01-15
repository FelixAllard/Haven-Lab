import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'motion/react';
import { Link } from 'react-router-dom'; // Import Link for routing

// Bootstrap CSS for styling
import 'bootstrap/dist/css/bootstrap.min.css';

const ProductPage = () => {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // State for search form
    const [searchTerm, setSearchTerm] = useState('');
    const [minPrice, setMinPrice] = useState('');
    const [maxPrice, setMaxPrice] = useState('');
    const [available, setAvailable] = useState(false);

    // Fetch products based on search parameters
    const fetchProducts = async (params = '') => {
        try {
            setLoading(true);
            const response = await axios.get(`http://localhost:5158/gateway/api/ProxyProduct${params}`);
            setProducts(response.data.items);
        } catch (err) {
            setError(err.message);
        } finally {
            setLoading(false);
        }
    };

    // Initial fetch on component mount
    useEffect(() => {
        fetchProducts();
    }, []);

    // Handle search button click
    const handleSearch = () => {
        let query = '?';

        if (searchTerm) query += `Name=${encodeURIComponent(searchTerm)}&`;
        if (minPrice) query += `MinimumPrice=${encodeURIComponent(minPrice)}&`;
        if (maxPrice) query += `MaximumPrice=${encodeURIComponent(maxPrice)}&`;
        if (available) query += `Available=${available}&`;

        // Remove the trailing '&' or '?' if no parameters are added
        if (query.endsWith('&') || query.endsWith('?')) {
            query = query.slice(0, -1);
        }

        // Fetch products with the query string
        fetchProducts(query);
    };

    // Render loading or error states
    if (loading) {
        return <div className="text-center">Loading...</div>;
    }

    if (error) {
        return <div className="text-center text-danger">Error: {error}</div>;
    }

    return (
        <div className="container mt-5">
            <br /><br />
            <h1>Our Products</h1>
            <br />
            <div className="row">
                {/* Search Form on the Left */}
                <div className="col-md-3">
                    <div className="card p-3 mb-4">
                        <h5 className="mb-3">Search Products</h5>
                        <div className="form-group mb-3">
                            <label htmlFor="searchTerm">Search</label>
                            <input
                                type="text"
                                className="form-control"
                                id="searchTerm"
                                placeholder="Search term"
                                value={searchTerm}
                                onChange={(e) => setSearchTerm(e.target.value)}
                            />
                        </div>
                        <div className="form-group mb-3">
                            <label htmlFor="minPrice">Minimum Price</label>
                            <input
                                type="number"
                                className="form-control"
                                id="minPrice"
                                placeholder="Minimum price"
                                value={minPrice}
                                onChange={(e) => setMinPrice(e.target.value)}
                            />
                        </div>
                        <div className="form-group mb-3">
                            <label htmlFor="maxPrice">Maximum Price</label>
                            <input
                                type="number"
                                className="form-control"
                                id="maxPrice"
                                placeholder="Maximum price"
                                value={maxPrice}
                                onChange={(e) => setMaxPrice(e.target.value)}
                            />
                        </div>
                        <div className="form-group form-check mb-3">
                            <input
                                type="checkbox"
                                className="form-check-input"
                                id="available"
                                checked={available}
                                onChange={(e) => setAvailable(e.target.checked)}
                            />
                            <label className="form-check-label" htmlFor="available">
                                Available
                            </label>
                        </div>
                        <button className="btn btn-primary w-100" onClick={handleSearch}>
                            Search
                        </button>
                    </div>
                </div>

                {/* Product Cards on the Right */}
                <div className="col-md-9">
                    <div className="row">
                        {products.map((product, index) => (
                            <motion.div
                                className="col-md-4 mb-4"
                                key={product.id}
                                initial={{ opacity: 0, y: 50 }}
                                animate={{ opacity: 1, y: 0 }}
                                transition={{ duration: 0.5, delay: index * 0.1 }}
                            >
                                <div className="card shadow-sm border-light">
                                    <img
                                        src={product.images[0]?.src || 'https://via.placeholder.com/150'}
                                        className="card-img-top"
                                        alt={product.title}
                                        style={{ height: '200px', objectFit: 'cover' }}
                                    />
                                    <div className="card-body">
                                        <h5 className="card-title">{product.title}</h5>
                                        <div
                                            className="card-text"
                                            dangerouslySetInnerHTML={{ __html: product.bodyHtml }}
                                        />
                                        <p><strong>Quantity:</strong> {product.variants[0]?.inventoryQuantity}</p>
                                        <p><strong>Price:</strong> ${product.variants[0]?.price}</p>
                                        <motion.button
                                            className="btn btn-primary"
                                            whileHover={{ scale: 1.1 }} // Grow on hover
                                            transition={{ duration: 0.2 }}
                                        >
                                            <Link
                                                to={`/product/${product.id}`} // Pass the product ID as a URL parameter
                                                style={{ color: 'white', textDecoration: 'none' }}
                                            >
                                                View Product
                                            </Link>
                                        </motion.button>
                                    </div>
                                </div>
                            </motion.div>
                        ))}
                    </div>
                </div>
            </div>
        </div>
    );
};

export default ProductPage;
