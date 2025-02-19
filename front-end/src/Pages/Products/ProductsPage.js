import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { Link } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import './ProductsPage.css';
import { FaSearch } from 'react-icons/fa';
import { useAuth } from '../../AXIOS/AuthentificationContext';
import httpClient from '../../AXIOS/AXIOS';
import '../../Languages/i18n.js';
import { useTranslation } from 'react-i18next';
import HoverScaleWrapper from '../../Shared/HoverScaleWrapper';

const ProductPage = () => {
  const [products, setProducts] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [minPrice, setMinPrice] = useState('');
  const [maxPrice, setMaxPrice] = useState('');
  const [available, setAvailable] = useState(false);
  const [currentPage, setCurrentPage] = useState(1);
  const { t } = useTranslation('products');

  const itemsPerPage = 6;

  const { authToken } = useAuth();
  const isLoggedIn = !!authToken;

  const fetchProducts = async (params = '') => {
    try {
      setLoading(true);
      const response = await httpClient.get(
        `/gateway/api/ProxyProduct${params}`,
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
    setCurrentPage(1);
  };

  const handleClearFilters = () => {
    setSearchTerm('');
    setMinPrice('');
    setMaxPrice('');
    setAvailable(false);
    fetchProducts();
    setCurrentPage(1);
  };

  const handleKeyPress = (event) => {
    if (event.key === 'Enter') {
      handleSearch(event);
    }
  };

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
              <h5>{t('Filter Products')}</h5>
              <div className="form-group mb-3">
                <input
                  type="number"
                  className="form-control"
                  placeholder={t('Minimum Price')}
                  value={minPrice}
                  onChange={(e) => setMinPrice(e.target.value)}
                />
              </div>
              <div className="form-group mb-3">
                <input
                  type="number"
                  className="form-control"
                  placeholder={t('Maximum Price')}
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
                <label className="form-check-label">{t('Available')}</label>
              </div>
              <div className="d-flex justify-content-between w-100">
                <HoverScaleWrapper>
                  <button
                    className="btn btn-secondary w-48"
                    onClick={handleSearch}
                  >
                    {t('Apply Filter')}
                  </button>
                </HoverScaleWrapper>
                <HoverScaleWrapper>
                  <button
                    className="btn btn-outline-danger w-48"
                    onClick={handleClearFilters}
                  >
                    {t('Clear Filters')}
                  </button>
                </HoverScaleWrapper>
              </div>
            </div>
          </div>

          <div className="col-md-9">
            <div className="search-bar-container">
              <input
                type="text"
                className="form-control search-bar"
                placeholder={t('Search')}
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
                      <HoverScaleWrapper>
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
                            <p className="price">
                              ${product.variants[0]?.price}
                            </p>
                            <Link
                              to={`/product/${product.id}`}
                              className="btn btn-secondary btn-block"
                            >
                              {t('View Product')}
                            </Link>
                          </div>
                        </div>
                      </HoverScaleWrapper>
                    </motion.div>
                  ))}
                </div>
              </>
            )}
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProductPage;
