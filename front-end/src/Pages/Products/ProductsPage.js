import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './ProductsPage.css';
import { FaSearch } from 'react-icons/fa';
import { useNavigate } from 'react-router-dom';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const ProductPage = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [available, setAvailable] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const navigate = useNavigate();

  const itemsPerPage = 6;

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

  useEffect(() => {
    fetchProducts();
  }, []);

  const handleSearch = (event) => {
    if (event) event.preventDefault();
    let query = '?';
    if (searchTerm) query += `Name=${encodeURIComponent(searchTerm)}&`;
    if (minPrice) query += `MinimumPrice=${encodeURIComponent(minPrice)}&`;
    if (maxPrice) query += `MaximumPrice=${encodeURIComponent(maxPrice)}&`;
    if (available) query += `Available=${available}&`;
    if (query.endsWith('&') || query.endsWith('?')) {
      query = query.slice(0, -1);
    }
    fetchProducts(query);
    setCurrentPage(1); // Reset to first page on new search
  };

  const handleKeyPress = (event) => {
    if (event.key === 'Enter') {
      handleSearch(event);
    }
  };

  // Pagination Logic
  const totalPages = Math.ceil(products.length / itemsPerPage);
  const paginatedProducts = products.slice(
    (currentPage - 1) * itemsPerPage,
    currentPage * itemsPerPage,
  );

  return (
    <div className="products-page">
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
            <button className="btn btn-success mb-3 mt-4">
              <Link
                to={`/admin/product/create`}
                style={{ color: 'white', textDecoration: 'none' }}
              >
                Add Product
              </Link>
            </button>
          </div>
          <div className="col-md-9">
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

            {loading ? (
              <div className="text-center">Loading...</div>
            ) : error ? (
              <div className="alert alert-danger" role="alert">
                Error: {error}
              </div>
            ) : (
              <>
                <div className="row">
                  {paginatedProducts.map((product, index) => (
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

                {/* Pagination Controls */}
                {totalPages > 1 && (
                  <div className="pagination d-flex justify-content-center mt-4">
                    <button
                      className="btn btn-outline-secondary mx-1"
                      disabled={currentPage === 1}
                      onClick={() => setCurrentPage(currentPage - 1)}
                    >
                      Previous
                    </button>
                    {[...Array(totalPages)].map((_, i) => (
                      <button
                        key={i}
                        className={`btn mx-1 ${currentPage === i + 1 ? 'btn-secondary' : 'btn-outline-secondary'}`}
                        onClick={() => setCurrentPage(i + 1)}
                      >
                        {i + 1}
                      </button>
                    ))}
                    <button
                      className="btn btn-outline-secondary mx-1"
                      disabled={currentPage === totalPages}
                      onClick={() => setCurrentPage(currentPage + 1)}
                    >
                      Next
                    </button>
                  </div>
                )}
                <div className="mt-5"></div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductPage;
