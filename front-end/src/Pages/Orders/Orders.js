import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const OrderPage = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(9); // Number of items per page
  const navigate = useNavigate();

  // Fetch orders on component mount
  useEffect(() => {
    const fetchOrders = async () => {
      try {
        setLoading(true);
        setError(null); // Clear any previous errors
        const response = await axios.get(
          `${environment}/gateway/api/ProxyOrder`,
        );
        setOrders(response.data.items || []); // Ensure fallback to empty array if no items
      } catch (err) {
        if (err.response) {
          setError(
            `Server Error: ${err.response.status} - ${
              err.response.data.message ||
              'An error occurred while fetching orders'
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

    fetchOrders();
  }, []);

  // Pagination logic
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentOrders = orders.slice(indexOfFirstItem, indexOfLastItem); // Slice orders for current page
  const totalPages = Math.ceil(orders.length / itemsPerPage); // Calculate total pages

  // Handle page change
  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  const handleViewClick = (orderId) => {
    // Navigate to the order detail page
    navigate(`/orders/${orderId}`);
  };

  return (
    <div className="container mt-5">
      <h1 className="mb-4">
        <br></br>Orders
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
          {currentOrders.length > 0 ? (
            currentOrders.map((order, index) => (
              <motion.div
                className="col-md-4 mb-4"
                key={order.id}
                initial={{ opacity: 0, y: 50 }}
                animate={{ opacity: 1, y: 0 }}
                transition={{ duration: 0.5, delay: index * 0.1 }}
              >
                <div className="card shadow-sm border-light">
                  <div className="card-body">
                    <p>
                      <strong>Order ID:</strong> {order.id || 'N/A'}
                    </p>
                    <p>
                      <strong>Timestamp:</strong>{' '}
                      {order.created_at
                        ? new Date(order.created_at).toLocaleString()
                        : 'N/A'}
                    </p>
                    <button
                      onClick={() => handleViewClick(order.id)}
                      className="btn btn-primary mt-3"
                    >
                      View Details
                    </button>
                  </div>
                </div>
              </motion.div>
            ))
          ) : (
            <div className="text-center w-100">
              <p className="text-muted">No orders available.</p>
            </div>
          )}
        </div>
      )}

      {/* Pagination Controls */}
      {!error && orders.length > itemsPerPage && (
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

export default OrderPage;
