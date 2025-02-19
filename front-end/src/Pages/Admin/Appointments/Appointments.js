import React, { useEffect, useState } from 'react';
import { motion } from 'framer-motion';
import { useNavigate } from 'react-router-dom';
import { FaSearch } from 'react-icons/fa';
import 'bootstrap/dist/css/bootstrap.min.css';
import './Appointments.css';
import httpClient from '../../../AXIOS/AXIOS';

const Appointments = () => {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null); // General error state
  const [filterError, setFilterError] = useState(null); // Filter-specific error state
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(9);
  const navigate = useNavigate();

  const [titleSearch, setTitleSearch] = useState('');
  const [customerNameSearch, setCustomerNameSearch] = useState('');
  const [customerEmailSearch, setCustomerEmailSearch] = useState('');
  const [statusSearch, setStatusSearch] = useState('');
  const [startDateSearch, setStartDateSearch] = useState('');
  const [endDateSearch, setEndDateSearch] = useState('');

  const fetchAppointments = async (query = '') => {
    try {
      setLoading(true);
      setError(null); // Clear any previous errors
      const response = await httpClient.get(
        `/gateway/api/ProxyAppointment/appointments${query}`,
      );

      // Sort appointments by appointmentDate (earliest to oldest)
      const sortedAppointments = (response.data || []).sort((a, b) => {
        return new Date(a.appointmentDate) - new Date(b.appointmentDate);
      });

      setAppointments(sortedAppointments);
    } catch (err) {
      if (err.response) {
        // Server responded with a status code outside 2xx
        setError(
          `Server Error: ${err.response.status} - ${err.response.data.message || 'An error occurred'}`,
        );
      } else if (err.request) {
        // The request was made but no response was received
        setError(
          'Network Error: Unable to connect to the server. Please check your connection or try again later.',
        );
      } else {
        // Something else happened
        setError(`Error: ${err.message}`);
      }
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchAppointments();
  }, []);

  const handleSearch = (e) => {
    e.preventDefault();

    // Clear previous filter errors
    setFilterError(null);

    // Validate year length for start date
    if (startDateSearch) {
      const startYear = new Date(startDateSearch).getFullYear().toString();
      if (startYear.length !== 4) {
        setFilterError('Year in start date must be exactly 4 characters.');
        return;
      }
    }

    // Validate year length for end date
    if (endDateSearch) {
      const endYear = new Date(endDateSearch).getFullYear().toString();
      if (endYear.length !== 4) {
        setFilterError('Year in end date must be exactly 4 characters.');
        return;
      }
    }

    // Validate dates
    if (
      startDateSearch &&
      endDateSearch &&
      new Date(startDateSearch) > new Date(endDateSearch)
    ) {
      setFilterError('Start date cannot be after end date.');
      return;
    }

    const params = new URLSearchParams();

    if (titleSearch) params.append('Title', titleSearch);
    if (customerNameSearch) params.append('CustomerName', customerNameSearch);
    if (customerEmailSearch)
      params.append('CustomerEmail', customerEmailSearch);
    if (statusSearch) params.append('Status', statusSearch);
    if (startDateSearch) params.append('StartDate', startDateSearch);
    if (endDateSearch) params.append('EndDate', endDateSearch);

    const queryString = params.toString();
    fetchAppointments(queryString ? `?${queryString}` : '');
    setCurrentPage(1);
  };

  const handleClearFilters = () => {
    setTitleSearch('');
    setCustomerNameSearch('');
    setCustomerEmailSearch('');
    setStatusSearch('');
    setStartDateSearch('');
    setEndDateSearch('');
    setFilterError(null); // Clear filter errors
    fetchAppointments();
    setCurrentPage(1);
  };

  // Pagination logic
  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentAppointments = appointments.slice(
    indexOfFirstItem,
    indexOfLastItem,
  );
  const totalPages = Math.ceil(appointments.length / itemsPerPage);

  const handlePageChange = (pageNumber) => setCurrentPage(pageNumber);

  const handleViewClick = (appointmentId) => {
    navigate(`/admin/appointments/${appointmentId}`);
  };

  const handleCreateClick = () => {
    navigate('/admin/appointments/create');
  };

  return (
    <div className="appointments-page container mt-7">
      <h1 className="mb-4">Appointments</h1>

      {/* Show loading, error, or content */}
      {loading ? (
        <div className="text-center mt-5">Loading...</div>
      ) : error ? (
        <div className="alert alert-danger" role="alert">
          {error}
        </div>
      ) : (
        <div className="row">
          {/* Filters Sidebar */}
          <div className="col-md-3">
            <div className="filter-section p-4">
              <h5>Filter Appointments</h5>
              <form onSubmit={handleSearch}>
                <div className="form-group mb-3">
                  <input
                    type="text"
                    className="form-control"
                    placeholder="Customer Name"
                    value={customerNameSearch}
                    onChange={(e) =>
                      setCustomerNameSearch(e.target.value.slice(0, 50))
                    } // Limit to 50 characters
                    maxLength={50}
                  />
                </div>
                <div className="form-group mb-3">
                  <input
                    type="email"
                    className="form-control"
                    placeholder="Customer Email"
                    value={customerEmailSearch}
                    onChange={(e) =>
                      setCustomerEmailSearch(e.target.value.slice(0, 100))
                    } // Limit to 100 characters
                    maxLength={100}
                  />
                </div>
                <div className="form-group mb-3">
                  <label>Status</label>
                  <select
                    className="form-control"
                    value={statusSearch}
                    onChange={(e) => setStatusSearch(e.target.value)}
                  >
                    <option value="">Select Status</option>
                    <option value="Upcoming">Upcoming</option>
                    <option value="Finished">Finished</option>
                    <option value="Cancelled">Cancelled</option>
                  </select>
                </div>
                <div className="form-group mb-3">
                  <label>Start Date</label>
                  <input
                    type="datetime-local"
                    className="form-control"
                    value={startDateSearch}
                    onChange={(e) => setStartDateSearch(e.target.value)}
                  />
                </div>
                <div className="form-group mb-3">
                  <label>End Date</label>
                  <input
                    type="datetime-local"
                    className="form-control"
                    value={endDateSearch}
                    onChange={(e) => setEndDateSearch(e.target.value)}
                    min={startDateSearch}
                  />
                </div>
                <div className="d-flex justify-content-between gap-3">
                  <button type="submit" className="btn btn-primary flex-grow-1">
                    Apply Filters
                  </button>
                  <button
                    type="button"
                    className="btn btn-secondary flex-grow-1"
                    onClick={handleClearFilters}
                  >
                    Clear Filters
                  </button>
                </div>
                {/* Display filter-specific errors here */}
                {filterError && (
                  <div className="alert alert-danger mt-3" role="alert">
                    {filterError}
                  </div>
                )}
              </form>
            </div>
            <button
              className="btn btn-success mb-3 mt-4"
              onClick={handleCreateClick}
            >
              New Appointment
            </button>
          </div>

          {/* Main Content */}
          <div className="col-md-9">
            {/* Search Bar */}
            <div className="search-bar-container mb-4">
              <div className="input-group">
                <input
                  type="text"
                  className="form-control search-bar"
                  placeholder="Search by Title..."
                  value={titleSearch}
                  onChange={(e) => setTitleSearch(e.target.value.slice(0, 100))} // Limit to 100 characters
                  maxLength={100}
                  onKeyPress={(e) => e.key === 'Enter' && handleSearch(e)}
                />
                <button className="search-icon-button" onClick={handleSearch}>
                  <FaSearch />
                </button>
              </div>
            </div>

            {/* Appointments Grid */}
            <div className="row">
              {currentAppointments.length > 0 ? (
                currentAppointments.map((appointment, index) => (
                  <motion.div
                    className="col-md-4 mb-4"
                    key={appointment.appointmentId}
                    initial={{ opacity: 0, y: 50 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ duration: 0.5, delay: index * 0.1 }}
                  >
                    <div className="card">
                      <div className="card-body">
                        <p className="card-text">
                          <strong>Title:</strong> {appointment.title || 'N/A'}
                        </p>
                        <p className="card-text">
                          <strong>Date:</strong>{' '}
                          {new Date(
                            appointment.appointmentDate,
                          ).toLocaleString()}
                        </p>
                        <p className="card-text">
                          <strong>Customer:</strong>{' '}
                          {appointment.customerName || 'N/A'}
                        </p>
                        <button
                          onClick={() =>
                            handleViewClick(appointment.appointmentId)
                          }
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
                  <p className="text-muted">No appointments found.</p>
                </div>
              )}
            </div>

            {/* Pagination */}
            {appointments.length > itemsPerPage && (
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
                    className={`btn mx-1 ${currentPage === i + 1 ? 'btn-secondary' : 'btn-outline-secondary'}`}
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
      )}
    </div>
  );
};

export default Appointments;
