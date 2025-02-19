import React, { useEffect, useState } from 'react';
import { useParams, Link, useNavigate } from 'react-router-dom';
import { motion } from 'motion/react';
import httpClient from '../../../AXIOS/AXIOS';
import './OrderDetail.css';

const OrderDetail = () => {
    const { orderId } = useParams();
    const navigate = useNavigate();
    const [order, setOrder] = useState(null);

    useEffect(() => {
        const fetchOrderDetail = async () => {
            try {
                const response = await httpClient.get(`/gateway/api/ProxyOrder/${orderId}`);
                console.log('Fetched Order Data:', response.data);
                setOrder(response.data);
            } catch (err) {
                console.error('Error fetching order details:', err);
            }
        };

        fetchOrderDetail();
    }, [orderId]);

    if (!order) {
        return <div className="text-center">Loading order details...</div>;
    }

    return (
        <div className="container mt-6">
            <div className="card shadow-lg">
                <div className="card-body">
                    <h2 className="card-title display-4 text-center text-white">Order Details</h2>

                    {/* Order Summary */}
                    <div className="order-summary">
                        <div className="order-item"><strong>App ID:</strong> {order.app_id}</div>
                        <div className="order-item"><strong>Order Number:</strong> {order.order_number}</div>
                        <div className="order-item"><strong>Created At:</strong> {new Date(order.created_at).toLocaleString()}</div>
                        <div className="order-item">
                            <strong>Order Status:</strong>
                            {order.order_status_url ? (
                                <a href={order.order_status_url} target="_blank" rel="noopener noreferrer" className="btn btn-link">
                                    View Order Status
                                </a>
                            ) : 'N/A'}
                        </div>
                        <div className="order-item"><strong>Financial Status:</strong> {order.financial_status}</div>
                        <div className="order-item"><strong>Total Price:</strong> {order.total_price} {order.currency}</div>
                        <div className="order-item"><strong>Subtotal:</strong> {order.subtotal_price} {order.currency}</div>
                        <div className="order-item"><strong>Taxes:</strong> {order.total_tax === 0 ? 'No Tax' : order.total_tax} {order.currency}</div>
                        <div className="order-item"><strong>Payment Gateway:</strong> {order.payment_gateway_names?.join(', ') || 'N/A'}</div>
                    </div>

                    {/* Shipping Information */}
                    <h4 className="text-white mb-3">Shipping Information</h4>
                    <div className="shipping-info">
                        <div className="shipping-item"><strong>Name:</strong> {order.shipping_address?.name || 'N/A'}</div>
                        <div className="shipping-item"><strong>Address 1:</strong> {order.shipping_address?.address1 || 'N/A'}</div>
                        <div className="shipping-item"><strong>Address 2:</strong> {order.shipping_address?.address2 || 'N/A'}</div>
                        <div className="shipping-item"><strong>City:</strong> {order.shipping_address?.city || 'N/A'}</div>
                        <div className="shipping-item"><strong>Province:</strong> {order.shipping_address?.province || 'N/A'}</div>
                        <div className="shipping-item"><strong>Country:</strong> {order.shipping_address?.country || 'N/A'}</div>
                        <div className="shipping-item"><strong>Zip:</strong> {order.shipping_address?.zip || 'N/A'}</div>
                        <div className="shipping-item"><strong>Shipping Phone:</strong> {order.shipping_address?.phone || 'N/A'}</div>
                        <div className="shipping-item"><strong>Company:</strong> {order.shipping_address?.company || 'N/A'}</div>
                        <div className="shipping-item"><strong>Customer Accepts Marketing:</strong> {order.buyer_accepts_marketing ? 'Yes' : 'No'}</div>
                        <div className="shipping-item"><strong>Tags:</strong> {order.tags || 'No tags available'}</div>
                    </div>

                    {/* Buttons */}
                    <div className="d-flex justify-content-between mt-4">
                        <motion.button
                            className="btn btn-secondary"
                            whileHover={{ scale: 1.1 }}
                            transition={{ duration: 0.2 }}
                            onClick={() => navigate('/admin/orders')} // Change this path to your desired page
                        >
                            Back to Orders
                        </motion.button>

                        <motion.button
                            className="btn btn-primary"
                            whileHover={{ scale: 1.1 }}
                            transition={{ duration: 0.2 }}
                        >
                            <Link
                                to={`/admin/order/update/${order.id}`}
                                style={{ color: 'white', textDecoration: 'none' }}
                            >
                                Edit Order
                            </Link>
                        </motion.button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default OrderDetail;
