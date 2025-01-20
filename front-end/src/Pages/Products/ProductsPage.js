import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './ProductsPage.css';
import { FaSearch } from 'react-icons/fa';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const ProductPage = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [available, setAvailable] = useState(false);

  const fetchProducts = async (params = '') => {
    try {
      setLoading(true);
      const response = await axios.get(
        `${environment}/gateway/api/ProxyProduct${params}`,
      );
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

  const handleSearch = (event) => {
    if (event) event.preventDefault(); // Prevent any default form submission
    let query = '?';
    if (searchTerm) query += `Name=${encodeURIComponent(searchTerm)}&`;
    if (minPrice) query += `MinimumPrice=${encodeURIComponent(minPrice)}&`;
    if (maxPrice) query += `MaximumPrice=${encodeURIComponent(maxPrice)}&`;
    if (available) query += `Available=${available}&`;
    if (query.endsWith('&') || query.endsWith('?')) {
      query = query.slice(0, -1);
    }
    fetchProducts(query);
  };

  const handleKeyPress = (event) => {
    if (event.key === 'Enter') {
      handleSearch(event);
    }
  };

  if (error) {
    return <div className="text-center text-danger">Error: {error}</div>;
  }

  return (
    <div className="container mt-5">
      <div className="row mt-3">
        <div className="col-md-3">
          <div className="filter-section p-4">
            <h5>Filter Products</h5>
            <div className="form-group mb-3">
              <input
                type="number"
                className="form-control"
                placeholder="Minimum price"
                value={minPrice}
                onChange={(e) => setMinPrice(e.target.value)}
              />
            </div>
            <div className="form-group mb-3">
              <input
                type="number"
                className="form-control"
                placeholder="Maximum price"
                value={maxPrice}
                onChange={(e) => setMaxPrice(e.target.value)}
              />
            </div>
            <div className="form-check mb-3">
              <input
                type="checkbox"
                className="form-check-input"
                checked={available}
                onChange={(e) => setAvailable(e.target.checked)}
              />
              <label className="form-check-label">Available</label>
            </div>
            <button
              className="btn btn-secondary btn-block"
              onClick={handleSearch}
            >
              Apply Filter
            </button>
          </div>
        </div>
        <div className="col-md-9">
          {/* Search Bar at the Top */}
          <div className="search-bar-container">
            <input
              type="text"
              className="form-control search-bar"
              placeholder="Search"
              value={searchTerm}
              onChange={(e) => setSearchTerm(e.target.value)}
              onKeyPress={handleKeyPress}
            />
            <button className="search-icon-button" onClick={handleSearch}>
              <FaSearch />
            </button>
          </div>

          {/* Display Loading or Products */}
          {loading ? (
            <div className="text-center">Loading...</div>
          ) : error ? (
            <div className="text-center text-danger">Error: {error}</div>
          ) : (
            <div className="row">
              {products.map((product, index) => (
                <motion.div
                  className="col-md-4 mb-4"
                  key={product.id}
                  initial={{ opacity: 0, y: 50 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5, delay: index * 0.1 }}
                >
                  <div className="card product-card">
                    <img
                      src={
                        product.images[0]?.src ||
                        require('../../Shared/imageNotFound.jpg')
                      }
                      className="card-img-top"
                      alt={product.title}
                    />
                    <div className="card-body">
                      <h5 className="card-title">{product.title}</h5>
                      <p className="price">${product.variants[0]?.price}</p>
                      <Link
                        to={`/product/${product.id}`}
                        className="btn btn-secondary btn-block"
                      >
                        View Product
                      </Link>
                    </div>
                  </div>
                </motion.div>
              ))}
            </div>
          )}
        </div>
      </div>
    </div>
  );
};

export default ProductPage;
