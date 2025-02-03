import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';
import { FaSearch } from 'react-icons/fa';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const OrderPage = () => {
  const [orders, setOrders] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(9); // Number of items per page
  const navigate = useNavigate();
  
  const [customerNameSearch, setCustomerNameSearch] = useState('');
  const [statusSearch, setStatusSearch] = useState('');
  const [dateBeforeSearch, setDateBeforeSearch] = useState('');
  const [dateAfterSearch, setDateAfterSearch] = useState('');

  // Fetch orders with optional filters
  const fetchOrders = async (params = '') => {
    try {
      setLoading(true);
      const response = await axios.get(`${environment}/gateway/api/ProxyOrder${params}`);
      setOrders(response.data.items || []);
    } catch (err) {
      setError(err.message);
    } finally {
      setLoading(false);
    }
  };

  // Initial fetch on mount
  useEffect(() => {
    fetchOrders();
  }, []);
  
  const buildQueryParams = () => {
    const queryParams = new URLSearchParams();
    if (customerNameSearch) queryParams.append('CustomerName', customerNameSearch);
    if (dateBeforeSearch) queryParams.append('DateBefore', dateBeforeSearch);
    if (dateAfterSearch) queryParams.append('DateAfter', dateAfterSearch);
    if (statusSearch) queryParams.append('Status', statusSearch);
    return queryParams.toString() ? `?${queryParams.toString()}` : '';
  };

  const handleSearch = (event) => {
    if (event) event.preventDefault();
    fetchOrders(buildQueryParams());
  };

  const handleKeyPress = (event) => {
    if (event.key === 'Enter') {
      handleSearch(event);
    }
  };

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
    navigate(`/orders/${orderId}`);
  };

  return (
    <div className="container mt-5">
      <div className="row mt-3">
        {/* Sidebar Filters */}
        <div className="col-md-3">
          <div className="filter-section p-4">
            <h5>Filter Orders</h5>
            <div className="form-group mb-3">
              <label>Before Date:</label>
              <input
                type="datetime-local"
                className="form-control"
                value={dateBeforeSearch}
                onChange={(e) => setDateBeforeSearch(e.target.value)}
              />
            </div>
            <div className="form-group mb-3">
              <label>After Date:</label>
              <input
                type="datetime-local"
                className="form-control"
                value={dateAfterSearch}
                onChange={(e) => setDateAfterSearch(e.target.value)}
              />
            </div>
            <div className="form-group mb-3">
              <label>Status:</label>
              <select
                className="form-control"
                value={statusSearch}
                onChange={(e) => setStatusSearch(e.target.value)}
              >
                <option value="">Select Status</option>
                <option value="pending">Pending</option>
                <option value="authorized">Authorized</option>
                <option value="partially_paid">Partially Paid</option>
                <option value="paid">Paid</option>
                <option value="partially_refunded">Partially Refunded</option>
                <option value="refunded">Refunded</option>
                <option value="voided">Voided</option>
              </select>
            </div>
            <button className="btn btn-secondary btn-block" onClick={handleSearch}>
              Apply Filter
            </button>
          </div>
        </div>

        {/* Main Content */}
        <div className="col-md-9">
          {/* Search Bar */}
          <div className="search-bar-container d-flex mb-3">
            <input
              type="text"
              className="form-control"
              placeholder="Search Orders"
              value={customerNameSearch}
              onChange={(e) => setCustomerNameSearch(e.target.value)}
              onKeyPress={handleKeyPress}
            />
            <button className="btn btn-secondary ms-2" onClick={handleSearch}>
              <FaSearch />
            </button>
          </div>

          {/* Show loading, error, or content */}
      {loading ? (
        <div className="text-center mt-5">Loading...</div>
      ) : error ? (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      ) : orders.length === 0 ? (
        <div className="text-center mt-5">No Orders Found</div>
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
      </div>
          
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
    </div>
  );
};

export default OrderPage;