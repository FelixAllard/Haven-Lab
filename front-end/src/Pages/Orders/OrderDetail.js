import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';

const OrderDetail = () => {
  const { orderId } = useParams(); // Get the orderId from the URL
  const [order, setOrder] = useState(null);

  useEffect(() => {
    // Fetch the order details by orderId
    const fetchOrderDetail = async () => {
      try {
        const response = await axios.get(`http://localhost:5158/gateway/api/ProxyOrder/${orderId}`);
        console.log("Fetched Order Data:", response.data);
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
    <div className="container mt-4">
      <h2>Order Details</h2>
      <p><strong>App ID:</strong> {order.app_id}</p>
      <p><strong>Order Number:</strong> {order.order_number}</p>
      <p><strong>Created At:</strong> {new Date(order.created_at).toLocaleString()}</p>
      <p>
        <strong>Order Status:</strong>{' '}
        {order.order_status_url ? (
          <a href={order.order_status_url} target="_blank" rel="noopener noreferrer">
            View Order Status
          </a>
        ) : (
          'N/A'
        )}
      </p>
      <p><strong>Financial Status:</strong> {order.financial_status}</p>
      <p><strong>Total Price:</strong> {order.total_price} {order.currency}</p>
      <p><strong>Subtotal:</strong> {order.subtotal_price} {order.currency}</p>
      <p><strong>Taxes:</strong> {order.totalTax === 0 ? 'No Tax' : order.totalTax} {order.currency}</p>
      <p>
        <strong>Payment Gateway:</strong> {order.payment_gateway_names?.join(', ') || 'N/A'}
      </p>
    </div>
  );
};

export default OrderDetail;