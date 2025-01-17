import React, { useEffect, useState } from 'react';
import axios from 'axios';
import { motion } from 'framer-motion';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';

const Appointments = () => {
  const [appointments, setAppointments] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // Fetch appointments on component mount
  useEffect(() => {
    const fetchAppointments = async () => {
      try {
        const response = await axios.get('http://localhost:5158/gateway/api/ProxyAppointment/all');
        console.log(response.data);
        setAppointments(response.data || []);
      } catch (err) {
        if (err.response) {
          setError(
            `Server Error: ${err.response.status} - ${
              err.response.data.message || 'An error occurred while fetching appointments'
            }`
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
  return (
    <div className="container mt-4">
      <h1 className="mb-4"><br></br>Appointments</h1>
      <div className="row">
        {appointments.length > 0 ? (
          appointments.map((appointment, index) => (
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
                    <strong>Appointment ID:</strong> {appointment.appointmentId || 'N/A'}
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
    </div>
  );
};

export default Appointments;
