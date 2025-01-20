import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const PriceRules = () => {
  const [priceRules, setPriceRules] = useState([]); // Updated state name for clarity
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // Fetch price rules on component mount
  useEffect(() => {
    const fetchPriceRules = async () => {
      try {
        const response = await axios.get(
          `${environment}/gateway/api/ProxyPromo/PriceRules`,
        );
        setPriceRules(response.data.items || []); // Ensure fallback to empty array if no items
      } catch (err) {
        if (err.response) {
          setError(
            `Server Error: ${err.response.status} - ${
              err.response.data.message ||
              'An error occurred while fetching price rules'
            }`,
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

  // Render loading or error states
  if (loading) {
    return <div className="text-center mt-5">Loading...</div>;
  }

  if (error) {
    return <div className="text-center text-danger mt-5">Error: {error}</div>;
  }

  return (
    <div className="container mt-5">
      <h1 className="mb-4">
        <br />
        Promo Price Rules
      </h1>
      <div className="row">
        {priceRules.length > 0 ? (
          priceRules.map((rule, index) => (
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
                    onClick={() => navigate(`/promo/pricerules/${rule.id}`)} // Adjust the path based on your routing structure
                  >
                    View Promo
                  </button>
                </div>
              </div>
            </motion.div>
          ))
        ) : (
          <div className="text-center w-100">
            <p className="text-muted">No price rules available.</p>
          </div>
        )}
      </div>
    </div>
  );
};

export default PriceRules;
