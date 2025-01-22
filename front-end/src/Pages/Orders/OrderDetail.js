import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import axios from 'axios';
import { motion } from 'motion/react';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;
const OrderDetail = () => {
  const { orderId } = useParams(); // Get the orderId from the URL
  const [order, setOrder] = useState(null);

  useEffect(() => {
    // Fetch the order details by orderId
    const fetchOrderDetail = async () => {
      try {
        const response = await axios.get(
          `${environment}/gateway/api/ProxyOrder/${orderId}`,
        );
        console.log('Fetched Order Data:', response.data);
        setOrder(response.data);
      } catch (err) {
        console.error('Error fetching order details:', err);
      }
    };

    fetchOrderDetail();
  }, [orderId]); // Re-fetch data when orderId changes

  if (!order) {
    return <div className="text-center">Loading order details...</div>;
  }

  return (
    <div className="container mt-6">
      <h2>Order Details</h2>
      <p>
        <strong>App ID:</strong> {order.app_id}
      </p>
      <p>
        <strong>Order Number:</strong> {order.order_number}
      </p>
      <p>
        <strong>Created At:</strong>{' '}
        {new Date(order.created_at).toLocaleString()}
      </p>
      <p>
        <strong>Order Status:</strong>{' '}
        {order.order_status_url ? (
          <a
            href={order.order_status_url}
            target="_blank"
            rel="noopener noreferrer"
          >
            View Order Status
          </a>
        ) : (
          'N/A'
        )}
      </p>
      <p>
        <strong>Financial Status:</strong> {order.financial_status}
      </p>
      <p>
        <strong>Total Price:</strong> {order.total_price} {order.currency}
      </p>
      <p>
        <strong>Subtotal:</strong> {order.subtotal_price} {order.currency}
      </p>
      <p>
        <strong>Taxes:</strong>{' '}
        {order.total_tax === 0 ? 'No Tax' : order.total_tax} {order.currency}
      </p>
      <p>
        <strong>Payment Gateway:</strong>{' '}
        {order.payment_gateway_names?.join(', ') || 'N/A'}
      </p>

      {/* Additional Fields */}
      <p>
        <strong>Note:</strong> {order.note || 'No note added'}
      </p>
      <h4>Shipping Information</h4>
      <p>
        <strong>Name:</strong> {order.shipping_address?.name || 'N/A'}
      </p>
      <p>
        <strong>Address 1:</strong> {order.shipping_address?.address1 || 'N/A'}
      </p>
      <p>
        <strong>Address 2:</strong> {order.shipping_address?.address2 || 'N/A'}
      </p>
      <p>
        <strong>City:</strong> {order.shipping_address?.city || 'N/A'}
      </p>
      <p>
        <strong>Province:</strong> {order.shipping_address?.province || 'N/A'}
      </p>
      <p>
        <strong>Country:</strong> {order.shipping_address?.country || 'N/A'}
      </p>
      <p>
        <strong>Zip:</strong> {order.shipping_address?.zip || 'N/A'}
      </p>
      <p>
        <strong>Shipping Phone:</strong>{' '}
        {order.shipping_address?.phone || 'N/A'}
      </p>
      {/* <p><strong>Email:</strong> {order.email || 'N/A'}</p> */}
      <p>
        <strong>Company:</strong> {order.shipping_address?.company || 'N/A'}
      </p>
      <p>
        <strong>Customer Accepts Marketing:</strong>{' '}
        {order.buyer_accepts_marketing ? 'Yes' : 'No'}
      </p>
      <p>
        <strong>Tags:</strong> {order.tags || 'No tags available'}
      </p>

      <motion.button
        className="btn btn-secondary"
        whileHover={{ scale: 1.1 }}
        transition={{ duration: 0.2 }}
      >
        <Link
          to={`/admin/order/update/${order.id}`}
          style={{ color: 'white', textDecoration: 'none' }}
        >
          Edit
        </Link>
      </motion.button>
    </div>
  );
};

export default OrderDetail;
