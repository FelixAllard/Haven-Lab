import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const OrderPage = () => {
  const [orders, setOrders] = useState([]); // Updated state name for clarity
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // Fetch orders on component mount
  useEffect(() => {
    const fetchOrders = async () => {
      try {
        const response = await axios.get('http://localhost:5158/gateway/api/ProxyOrder');
        setOrders(response.data.items || []); // Ensure fallback to empty array if no items
      } catch (err) {
        if (err.response) {
          setError(
            `Server Error: ${err.response.status} - ${
              err.response.data.message || 'An error occurred while fetching orders'
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

    fetchOrders();
  }, []);

  // Render loading or error states
  if (loading) {
    return <div className="text-center mt-5">Loading...</div>;
  }

  if (error) {
    return <div className="text-center text-danger mt-5">Error: {error}</div>;
  }

  const handleViewClick = (orderId) => {
    // Navigate to the order detail page
    navigate(`/orders/${orderId}`);
  };

  return (
    <div className="container mt-4">
      <h1 className="mb-4"><br></br>Orders</h1>
      <div className="row">
        {orders.length > 0 ? (
          orders.map((order, index) => (
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
                    <strong>Apple ID:</strong> {order.app_id || 'N/A'}
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
    </div>
  );
};

export default OrderPage;
