import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'motion/react';

// Bootstrap CSS for card styling
import 'bootstrap/dist/css/bootstrap.min.css';

const OrderPage = () => {
    const [products, setOrders] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    // Fetch orders on component mount
    useEffect(() => {
        const fetchOrders = async () => {
            try {
                const response = await axios.get('http://localhost:5158/gateway/api/ProxyOrder');
                setOrders(response.data.items);
            } catch (err) {
                // Check for specific error types
                if (err.response) {
                    // The server responded with a status other than 2xx
                    setError(`Server Error: ${err.response.status} - ${err.response.data.message || 'An error occurred while fetching orders'}`);
                } else if (err.request) {
                    // The request was made but no response was received
                    setError('Network Error: No response received from the server.');
                } else {
                    // Something went wrong during setting up the request
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
                        initial={{opacity: 0, y: 50}}
                        animate={{opacity: 1, y: 0}}
                        transition={{duration: 0.5, delay: index * 0.1}}
                    >
                        <div className="card shadow-sm border-light">
                            <div className="card-body">
                                <button>View</button>
                                <p><strong>Apple Id:</strong> {order.appId}</p>
                                <p><strong>Timestamp:</strong> {order.createdAt}</p>
                                <p>
                                    <strong>Details:</strong>
                                    {order.lineItems && order.lineItems.length > 0 ? (
                                        order.lineItems.map((item, index) => (
                                        <div key={index}>
                                            <p>
                                            <strong>Item {index + 1}:</strong>
                                            <br />
                                            Fulfillable Quantity: {item.fulfillableQuantity ?? 'N/A'}
                                            <br />
                                            Fulfillment Service: {item.fulfillmentService ?? 'manual'}
                                            </p>
                                        </div>
                                        ))
                                    ) : (
                                        <p>No line items available</p>
                                    )}
                                </p>
                            </div>
                        </div>
                    </motion.div>
                ))}
            </div>
        </div>
    );
};

export default OrderPage;
