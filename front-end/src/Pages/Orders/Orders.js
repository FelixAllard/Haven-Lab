import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'motion/react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const OrderPage = () => {
  const [products, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();  

  // Fetch orders on component mount
  useEffect(() => { 
    const fetchOrders = async () => {
      try {
        const response = await axios.get('http://localhost:5158/gateway/api/ProxyOrder');
        setOrders(response.data.items);
      } catch (err) {
        if (err.response) {
          setError(`Server Error: ${err.response.status} - ${err.response.data.message || 'An error occurred while fetching orders'}`);
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
    return <div className="text-center">Loading...</div>;
  }

  if (error) {
    return <div className="text-center text-danger">Error: {error}</div>;
  }

  const handleViewClick = (orderId) => {
    // Use navigate to programmatically navigate to the order details page
    navigate(`/orders/${orderId}`);
  };

  return (
    <div className="container mt-4">
      <br/><br/>
      <h1>Orders</h1>
      <br/>
      <div className="row">
        {products.map((order, index) => (
          <motion.div
            className="col-md-4 mb-4"
            key={order.id}
            initial={{ opacity: 0, y: 50 }}
            animate={{ opacity: 1, y: 0 }}
            transition={{ duration: 0.5, delay: index * 0.1 }}
          >
            <div className="card shadow-sm border-light">
              <div className="card-body">
                {/* Use Link to navigate to the order detail page */}
                <button onClick={() => handleViewClick(order.id)} className="btn btn-primary">View</button>
                <p><strong>Apple Id:</strong> {order.appId}</p>
                <p><strong>Timestamp:</strong> {order.createdAt}</p>
              </div>
            </div>
          </motion.div>
        ))}
      </div>
    </div>
  );
};

export default OrderPage;
