import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { useParams, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';

const OrderUpdatePage = () => {
  const { orderId } = useParams(); // Retrieve orderId from URL
  const navigate = useNavigate();

  const [order, setOrder] = useState({
    note: '',
    email: '',
    phone: '',
    buyer_accepts_marketing: false,
    tags: '',
    shipping_address: {
      address1: '',
      address2: '',
      city: '',
      company: '',
      country: '',
      country_code: '',
      first_name: '',
      last_name: '',
      phone: '',
      province: '',
      province_code: '',
      zip: '',
    },
  });

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [successMessage, setSuccessMessage] = useState('');

  // Fetch order details on mount
  useEffect(() => {
    const fetchOrderDetails = async () => {
      try {
        const response = await axios.get(`http://localhost:5158/gateway/api/ProxyOrder/${orderId}`);
        setOrder(response.data);
      } catch (err) {
        setError('Error fetching order details.');
      } finally {
        setLoading(false);
      }
    };
    fetchOrderDetails();
  }, [orderId]);

  // Handle form submission
  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      console.log(order);
      const response = await axios.put(`http://localhost:5158/gateway/api/ProxyOrder/${orderId}`, order);
      if (response.status === 200) {
        setSuccessMessage('Order updated successfully!');
        setTimeout(() => navigate(`/orders/${orderId}`), 2000); // Redirect to orders page after 2 seconds
      } else {
        console.error('Unexpected Response:', response);
      }
    } catch (err) {
      setError('Error updating order.');
      console.error('Update Error:', err);
    }
  };

  // Handle input changes
  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    if (name.startsWith('shipping_address.')) {
      const key = name.split('.')[1];
      setOrder({
        ...order,
        shipping_address: {
          ...order.shipping_address,
          [key]: value,
        },
      });
    } else {
      setOrder({
        ...order,
        [name]: type === 'checkbox' ? checked : value,
      });
    }
  };

  if (loading) {
    return <div className="text-center mt-5">Loading...</div>;
  }

  if (error) {
    return <div className="text-center text-danger mt-5">{error}</div>;
  }

  return (
<div className="container mt-4">
  <h1 className="mb-4">Update Order</h1>
  <h4>As being a very primitive version, this page shows the update functionnality. 
    Make sure when you update the fields, write something CORRECT, for example, country can be Canada, China, United-States, etc.
    Do not write something "Canadaaa"! Same for the other fields.
    Otherwise, it will show error message, since Shopify has a strict limitation for input on its side.
    A bug story should be added for the page to implement PRETTY error handling! 
    So that messages are visible for notifying and guiding users.     ---Regine</h4>
  <form onSubmit={handleSubmit}>
    <div className="mb-3">
      <label htmlFor="note" className="form-label">Note</label>
      <textarea
        id="note"
        name="note"
        className="form-control"
        value={order.note || ''}
        onChange={handleChange}
        maxLength="500"
        required  // Required field
      />
      <small>Max 500 characters</small>
    </div>
    <div className="mb-3">
      <label className="form-label">Accepts Marketing</label>
      <div className="form-check">
        <input
          type="checkbox"
          id="buyer_accepts_marketing"
          name="buyer_accepts_marketing"
          className="form-check-input"
          checked={order.buyer_accepts_marketing || false}
          onChange={handleChange}
        />
        <label htmlFor="buyer_accepts_marketing" className="form-check-label">Yes</label>
      </div>
    </div>
    <div className="mb-3">
      <label htmlFor="tags" className="form-label">Tags</label>
      <input
        type="text"
        id="tags"
        name="tags"
        className="form-control"
        value={order.tags || ''}
        onChange={handleChange}
        maxLength="100"
        required  // Required field
      />
      <small>Max 100 characters</small>
    </div>
    <h3>Shipping Information</h3>
    <div className="mb-3">
      <label htmlFor="shipping_address.name" className="form-label">Name</label>
      <input
        type="text"
        id="shipping_address.name"
        name="shipping_address.name"
        className="form-control"
        value={order.shipping_address.name || ''}
        onChange={handleChange}
        maxLength="150"
        required  // Required field
      />
      <small>Max 150 characters</small>
    </div>
    <div className="mb-3">
      <label htmlFor="shipping_address.address1" className="form-label">Address 1</label>
      <input
        type="text"
        id="shipping_address.address1"
        name="shipping_address.address1"
        className="form-control"
        value={order.shipping_address.address1 || ''}
        onChange={handleChange}
        maxLength="200"
        required  // Required field
      />
      <small>Max 200 characters</small>
    </div>
    <div className="mb-3">
      <label htmlFor="shipping_address.city" className="form-label">City</label>
      <input
        type="text"
        id="shipping_address.city"
        name="shipping_address.city"
        className="form-control"
        value={order.shipping_address.city || ''}
        onChange={handleChange}
        maxLength="100"
        required  // Required field
      />
      <small>Max 100 characters</small>
    </div>
    <div className="mb-3">
      <label htmlFor="shipping_address.zip" className="form-label">Zip</label>
      <input
        type="text"
        id="shipping_address.zip"
        name="shipping_address.zip"
        className="form-control"
        value={order.shipping_address.zip || ''}
        onChange={handleChange}
        maxLength="15"
        required  // Required field
      />
      <small>Max 15 characters</small>
    </div>
    <div className="mb-3">
      <label htmlFor="shipping_address.province" className="form-label">Province</label>
      <input
        type="text"
        id="shipping_address.province"
        name="shipping_address.province"
        className="form-control"
        value={order.shipping_address.province || ''}
        onChange={handleChange}
        maxLength="100"
        required  // Required field
      />
      <small>Max 100 characters</small>
    </div>
    <div className="mb-3">
      <label htmlFor="shipping_address.country" className="form-label">Country</label>
      <input
        type="text"
        id="shipping_address.country"
        name="shipping_address.country"
        className="form-control"
        value={order.shipping_address.country || ''}
        onChange={handleChange}
        maxLength="100"
        required  // Required field
      />
      <small>Max 100 characters</small>
    </div>
    <div className="mb-3">
      <label htmlFor="shipping_address.phone" className="form-label">Shipping Phone</label>
      <input
        type="text"
        id="shipping_address.phone"
        name="shipping_address.phone"
        className="form-control"
        value={order.shipping_address.phone || ''}
        onChange={handleChange}
        maxLength="15"
        required  // Required field
      />
      <small>Max 15 characters</small>
    </div>
    <div className="mb-3">
      <label htmlFor="shipping_address.company" className="form-label">Company</label>
      <input
        type="text"
        id="shipping_address.company"
        name="shipping_address.company"
        className="form-control"
        value={order.shipping_address.company || ''}
        onChange={handleChange}
        maxLength="100"
      />
      <small>Max 100 characters</small>
    </div>
    <button type="submit" className="btn btn-primary">Update Order</button>
    <button
      type="button"
      className="btn btn-secondary ms-3"
      onClick={() => navigate(`/orders/${orderId}`)}
    >
      Cancel
    </button>
  </form>
  <br />
  {successMessage && (
    <div className="alert alert-success">{successMessage}</div>
  )}
</div>

  );
};  

export default OrderUpdatePage;
