import React, { useEffect, useState } from 'react';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useParams, useNavigate } from 'react-router-dom';
import httpClient from '../../AXIOS/AXIOS';
const AppointmentDetail = () => {
  const { appointmentId } = useParams(); // Get the appointmentId from the URL
  const [appointment, setAppointment] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // Fetch the appointment details when the component is mounted
  useEffect(() => {
    const fetchAppointmentDetails = async () => {
      try {
        const response = await httpClient.get(
          `/gateway/api/ProxyAppointment/${appointmentId}`,
        );
        setAppointment(response.data);
      } catch (err) {
        if (err.response) {
          setError(
            `Server Error: ${err.response.status} - ${
              err.response.data.message ||
              'An error occurred while fetching appointment details'
            }`,
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

    fetchAppointmentDetails();
  }, [appointmentId]);

  // Handle delete
  const handleDelete = async () => {
    try {
      const response = await httpClient.delete(
        `/gateway/api/ProxyAppointment/${appointmentId}`,
      );
      if (response.status === 200) {
        // Redirect to appointments list after deleting
        navigate('/appointments');
      }
    } catch (err) {
      setError(`Error deleting appointment: ${err.message}`);
    }
  };

  // Handle update navigation
  const handleUpdate = () => {
    navigate(`/appointments/update/${appointmentId}`);
  };

  // Render loading or error states
  if (loading) {
    return <div className="text-center mt-5">Loading...</div>;
  }

  if (error) {
    return <div className="text-center text-danger mt-5">Error: {error}</div>;
  }

  return (
    <div className="container mt-7">
      <h1 className="mb-4">Appointment Details</h1>
      {appointment ? (
        <div className="card shadow-sm border-light">
          <div className="card-body">
            <h5 className="card-title">Title: {appointment.title}</h5>
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
            <p>
              <strong>Customer Email:</strong>{' '}
              {appointment.customerEmail || 'N/A'}
            </p>
            <p>
              <strong>Description:</strong> {appointment.description || 'N/A'}
            </p>
            <p>
              <strong>Status:</strong> {appointment.status || 'N/A'}
            </p>
            <p>
              <strong>Created At:</strong>{' '}
              {appointment.createdAt
                ? new Date(appointment.createdAt).toLocaleString()
                : 'N/A'}
            </p>

            <button
              className="btn btn-secondary mt-3"
              onClick={() => navigate('/appointments')}
            >
              Back
            </button>

            <button
              className="btn btn-warning mt-3 ml-3"
              onClick={handleUpdate}
            >
              Update
            </button>

            <button className="btn btn-danger mt-3 ml-3" onClick={handleDelete}>
              Delete
            </button>
          </div>
        </div>
      ) : (
        <div className="text-center w-100">
          <p className="text-muted">No appointment details available.</p>
        </div>
      )}
    </div>
  );
};

export default AppointmentDetail;
