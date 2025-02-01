import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const PriceRules = () => {
  const [priceRules, setPriceRules] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1); // Track current page
  const [itemsPerPage] = useState(9); // Number of items per page (set to 9)
  const navigate = useNavigate();

  // Fetch price rules on component mount
  useEffect(() => {
    const fetchPriceRules = async () => {
      try {
        setLoading(true);
        setError(null); // Clear any previous errors
        const response = await axios.get(
          `${environment}/gateway/api/ProxyPromo/PriceRules`
        );
        setPriceRules(response.data.items || []); // Ensure fallback to empty array if no items
      } catch (err) {
        if (err.response) {
          setError(
            `Server Error: ${err.response.status} - ${
              err.response.data.message ||
              'An error occurred while fetching price rules'
            }`
          );
        } else if (err.request) {
          setError('Network Error: No response received from the server.');
        } else {
          setError(`Error: ${err.message}`);
        }
      } finally {
        setLoading(false);
      }
    };

    fetchPriceRules();
  }, []);

  // Pagination logic
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentPriceRules = priceRules.slice(indexOfFirstItem, indexOfLastItem); // Slice price rules for current page
  const totalPages = Math.ceil(priceRules.length / itemsPerPage); // Calculate total pages

  // Handle page change
  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  return (
    <div className="container mt-5">
      <h1 className="mb-4">
        <br />
        Promo Price Rules
      </h1>

      {/* Show loading, error, or content */}
      {loading ? (
        <div className="text-center mt-5">Loading...</div>
      ) : error ? (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      ) : (
        <div className="row">
          {currentPriceRules.length > 0 ? (
            currentPriceRules.map((rule, index) => (
              <motion.div
                className="col-md-4 mb-4"
                key={rule.id}
                initial={{ opacity: 0, y: 50 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: index * 0.1 }}
              >
                <div className="card shadow-sm border-light">
                  <div className="card-body">
                    <p>
                      <strong>Price Rule ID:</strong> {rule.id || 'N/A'}
                    </p>
                    <p>
                      <strong>Title:</strong> {rule.title || 'N/A'}
                    </p>
                    <p>
                      <strong>Savings:</strong> {rule.value || 'N/A'}
                    </p>
                    <p>
                      <strong>Created At:</strong>{' '}
                      {rule.created_at
                        ? new Date(rule.created_at).toLocaleString()
                        : 'N/A'}
                    </p>
                    <button
                      className="btn btn-primary mt-3"
                      onClick={() => navigate(`/promo/pricerules/${rule.id}`)}
                    >
                      View Promo
                    </button>
                  </div>
                </div>
              </motion.div>
            ))
          ) : (
            <div className="alert alert-danger" role="alert">
              No price rules available.
            </div>
          )}
        </div>
      )}

      {/* Pagination Controls */}
      {!error && priceRules.length > itemsPerPage && (
        <div className="pagination d-flex justify-content-center mt-4">
          <button
            className="btn btn-outline-secondary mx-1"
            disabled={currentPage === 1}
            onClick={() => handlePageChange(currentPage - 1)}
          >
            Previous
          </button>
          {[...Array(totalPages)].map((_, i) => (
            <button
              key={i}
              className={`btn mx-1 ${
                currentPage === i + 1
                  ? 'btn-secondary'
                  : 'btn-outline-secondary'
              }`}
              onClick={() => handlePageChange(i + 1)}
            >
              {i + 1}
            </button>
          ))}
          <button
            className="btn btn-outline-secondary mx-1"
            disabled={currentPage === totalPages}
            onClick={() => handlePageChange(currentPage + 1)}
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
};

export default PriceRules;