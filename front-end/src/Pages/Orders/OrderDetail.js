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
      
      <p><strong>Apple Id:</strong> {order.appId}</p>
      <p><strong>Order Number:</strong> {order.orderNumber}</p>
      <p><strong>Timestamp:</strong> {new Date(order.createdAt).toLocaleString()}</p>
      <p><strong>Order Status:</strong> {order.orderStatusUrl ? <a href={order.orderStatusUrl} target="_blank" rel="noopener noreferrer">View Order Status</a> : 'N/A'}</p>
      <p><strong>Payment Status:</strong> {order.financialStatus}</p>
      <p><strong>Total Price:</strong> {order.totalPrice} {order.currency}</p>
      <p><strong>Subtotal:</strong> {order.subtotalPrice} {order.currency}</p>
      <p><strong>Taxes:</strong> {order.totalTax === 0 ? 'No Tax' : order.totalTax} {order.currency}</p>
    </div>
  );
};

export default OrderDetail;
