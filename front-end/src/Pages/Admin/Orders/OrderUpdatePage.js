import React, { useEffect, useState } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import httpClient from '../../../AXIOS/AXIOS';

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
        const response = await httpClient.get(`/gateway/api/ProxyOrder/${orderId}`);
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
      const response = await httpClient.put(`/gateway/api/ProxyOrder/${orderId}`, order);
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

        <form onSubmit={handleSubmit}>
          {/* Note and Tags */}
          <div className="mb-3">
            <label htmlFor="note" className="form-label">
              Note
            </label>
            <textarea
                id="note"
                name="note"
                className="form-control"
                value={order.note || ''}
                onChange={handleChange}
                maxLength="500"
                required
                placeholder="Add any notes about the order."
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
              <label htmlFor="buyer_accepts_marketing" className="form-check-label">
                Yes
              </label>
            </div>
          </div>

          <div className="mb-3">
            <label htmlFor="tags" className="form-label">
              Tags
            </label>
            <input
                type="text"
                id="tags"
                name="tags"
                className="form-control"
                value={order.tags || ''}
                onChange={handleChange}
                maxLength="100"
                required
                placeholder="Enter relevant tags (e.g., 'VIP', 'Urgent')."
            />
            <small>Max 100 characters</small>
          </div>

          {/* Shipping Information */}
          <h3 className="mt-4">Shipping Information</h3>

          <div className="row">
            {/* Name */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.name" className="form-label">
                  Full Name
                </label>
                <input
                    type="text"
                    id="shipping_address.name"
                    name="shipping_address.name"
                    className="form-control"
                    value={order.shipping_address.name || ''}
                    onChange={handleChange}
                    maxLength="150"
                    required
                    placeholder="Enter recipient's full name."
                />
                <small>Max 150 characters</small>
              </div>
            </div>

            {/* Address 1 */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.address1" className="form-label">
                  Address 1
                </label>
                <input
                    type="text"
                    id="shipping_address.address1"
                    name="shipping_address.address1"
                    className="form-control"
                    value={order.shipping_address.address1 || ''}
                    onChange={handleChange}
                    maxLength="200"
                    required
                    placeholder="Street address, P.O. box, company name, etc."
                />
                <small>Max 200 characters</small>
              </div>
            </div>

            {/* Address 2 */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.address2" className="form-label">
                  Address 2 (Optional)
                </label>
                <input
                    type="text"
                    id="shipping_address.address2"
                    name="shipping_address.address2"
                    className="form-control"
                    value={order.shipping_address.address2 || ''}
                    onChange={handleChange}
                    maxLength="200"
                    placeholder="Apartment, suite, unit, building, floor, etc."
                />
                <small>Max 200 characters</small>
              </div>
            </div>

            {/* City */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.city" className="form-label">
                  City
                </label>
                <input
                    type="text"
                    id="shipping_address.city"
                    name="shipping_address.city"
                    className="form-control"
                    value={order.shipping_address.city || ''}
                    onChange={handleChange}
                    maxLength="100"
                    required
                    placeholder="Enter city."
                />
                <small>Max 100 characters</small>
              </div>
            </div>

            {/* Zip */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.zip" className="form-label">
                  Zip Code
                </label>
                <input
                    type="text"
                    id="shipping_address.zip"
                    name="shipping_address.zip"
                    className="form-control"
                    value={order.shipping_address.zip || ''}
                    onChange={handleChange}
                    maxLength="15"
                    required
                    placeholder="Enter zip code."
                />
                <small>Max 15 characters</small>
              </div>
            </div>

            {/* Province */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.province" className="form-label">
                  Province
                </label>
                <input
                    type="text"
                    id="shipping_address.province"
                    name="shipping_address.province"
                    className="form-control"
                    value={order.shipping_address.province || ''}
                    onChange={handleChange}
                    maxLength="100"
                    required
                    placeholder="Enter province or state."
                />
                <small>Max 100 characters</small>
              </div>
            </div>

            {/* Country */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.country" className="form-label">
                  Country
                </label>
                <input
                    type="text"
                    id="shipping_address.country"
                    name="shipping_address.country"
                    className="form-control"
                    value={order.shipping_address.country || ''}
                    onChange={handleChange}
                    maxLength="100"
                    required
                    placeholder="Enter country."
                />
                <small>Max 100 characters</small>
              </div>
            </div>

            {/* Phone */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.phone" className="form-label">
                  Shipping Phone
                </label>
                <input
                    type="text"
                    id="shipping_address.phone"
                    name="shipping_address.phone"
                    className="form-control"
                    value={order.shipping_address.phone || ''}
                    onChange={handleChange}
                    maxLength="15"
                    required
                    placeholder="Enter phone number."
                />
                <small>Max 15 characters</small>
              </div>
            </div>

            {/* Company */}
            <div className="col-md-6">
              <div className="mb-3">
                <label htmlFor="shipping_address.company" className="form-label">
                  Company (Optional)
                </label>
                <input
                    type="text"
                    id="shipping_address.company"
                    name="shipping_address.company"
                    className="form-control"
                    value={order.shipping_address.company || ''}
                    onChange={handleChange}
                    maxLength="100"
                    placeholder="Enter company name."
                />
                <small>Max 100 characters</small>
              </div>
            </div>
          </div>

          {/* Action Buttons */}
          <div className="d-flex justify-content-between mt-4">
            <button
                type="button"
                className="btn btn-secondary"
                onClick={() => navigate(`/admin/orders/${orderId}`)}
            >
              Cancel
            </button>
            <button type="submit" className="btn btn-primary">
              Update Order
            </button>
          </div>
        </form>

        {/* Success/Error Message */}
        {successMessage && (
            <div className="alert alert-success mt-3">{successMessage}</div>
        )}
        {error && <div className="alert alert-danger mt-3">{error}</div>}
      </div>
  );
};

export default OrderUpdatePage;
