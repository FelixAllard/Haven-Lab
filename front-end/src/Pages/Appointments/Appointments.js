import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const Appointments = () => {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(9);
  const navigate = useNavigate();

  // Fetch appointments on component mount
  useEffect(() => {
    const fetchAppointments = async () => {
      try {
        const response = await axios.get(`${environment}/gateway/api/ProxyAppointment/all`);
        setAppointments(response.data || []);
      } catch (err) {
        if (err.response) {
          setError(
            `Server Error: ${err.response.status} - ${err.response.data.message || 'An error occurred'}`,
          );
        } else if (err.request) {
          setError('Network Error: No response received from the server.');
        } else {
          setError(`Error: ${err.message}`);
        }
      } finally {
        setLoading(false);
      }
    };

    fetchAppointments();
  }, []);

  const indexOfLastItem = currentPage * itemsPerPage;
  const indexOfFirstItem = indexOfLastItem - itemsPerPage;
  const currentAppointments = appointments.slice(indexOfFirstItem, indexOfLastItem);
  const totalPages = Math.ceil(appointments.length / itemsPerPage);

  const handlePageChange = (pageNumber) => {
    setCurrentPage(pageNumber);
  };

  // Render loading or error states
  if (loading) {
    return <div className="text-center mt-5">Loading...</div>;
  }

  if (error) {
    return <div className="text-center text-danger mt-5">Error: {error}</div>;
  }

  const handleViewClick = (appointmentId) => {
    navigate(`/appointments/${appointmentId}`);
  };

  const handleCreateClick = () => {
    navigate('/appointments/create');
  };

  return (
    <div className="container mt-7">
      <h1 className="mb-4">Appointments</h1>
      <button className="btn btn-success mb-3" onClick={handleCreateClick}>
        Create Appointment
      </button>
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
              <div className="card shadow-sm border-light">
                <div className="card-body">
                  <p>
                    <strong>Title:</strong> {appointment.title || 'N/A'}
                  </p>
                  <p>
                    <strong>Date:</strong>{' '}
                    {appointment.appointmentDate
                      ? new Date(appointment.appointmentDate).toLocaleString()
                      : 'N/A'}
                  </p>
                  <p>
                    <strong>Customer Name:</strong>{' '}
                    {appointment.customerName || 'N/A'}
                  </p>
                  <button
                    onClick={() => handleViewClick(appointment.appointmentId)}
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
            <p className="text-muted">No appointments available.</p>
          </div>
        )}
      </div>

      {/* Pagination Controls */}
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
              className={`btn mx-1 ${
                currentPage === i + 1 ? 'btn-secondary' : 'btn-outline-secondary'
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
  );
};

export default Appointments;