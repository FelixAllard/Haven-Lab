import React, { useState } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useNavigate } from 'react-router-dom';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const AppointmentCreate = () => {
  const [appointment, setAppointment] = useState({
    title: '',
    customerName: '',
    customerEmail: '',
    appointmentDate: '',
    description: '',
    status: 'Upcoming',
    createdAt: '',  // Initialize the createdAt field
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  const handleCreate = async () => {
    setLoading(true);

    // Set the createdAt field to the current date and time
    const currentDate = new Date().toISOString(); // Get the current date and time in ISO format
    const appointmentToCreate = { ...appointment, createdAt: currentDate };

    try {
      const response = await axios.post(`${environment}/gateway/api/ProxyAppointment`, appointmentToCreate);
      if (response.status === 201) {
        navigate('/appointments'); // Redirect to appointments list after successful creation
      }
    } catch (err) {
      setError('Failed to create appointment');
    } finally {
      setLoading(false);
    }
  };

  const handleCancel = () => {
    navigate('/appointments'); // Navigate back to appointments list
  };

  return (
    <div className="container mt-7">
      <h1>Create Appointment</h1>
      {error && <div className="alert alert-danger">{error}</div>}
      <div className="card shadow-sm border-light">
        <div className="card-body">
          <form>
            {/* Appointment Title */}
            <div className="form-group">
              <label>Title</label>
              <input
                type="text"
                className="form-control"
                value={appointment.title}
                onChange={(e) => setAppointment({ ...appointment, title: e.target.value })}
              />
            </div>

            {/* Customer Name */}
            <div className="form-group">
              <label>Customer Name</label>
              <input
                type="text"
                className="form-control"
                value={appointment.customerName}
                onChange={(e) => setAppointment({ ...appointment, customerName: e.target.value })}
              />
            </div>

            {/* Customer Email */}
            <div className="form-group">
              <label>Customer Email</label>
              <input
                type="email"
                className="form-control"
                value={appointment.customerEmail}
                onChange={(e) => setAppointment({ ...appointment, customerEmail: e.target.value })}
              />
            </div>

            {/* Appointment Date */}
            <div className="form-group">
              <label>Date</label>
              <input
                type="datetime-local"
                className="form-control"
                value={appointment.appointmentDate}
                onChange={(e) => setAppointment({ ...appointment, appointmentDate: e.target.value })}
              />
            </div>

            {/* Description */}
            <div className="form-group">
              <label>Description</label>
              <textarea
                className="form-control"
                value={appointment.description}
                onChange={(e) => setAppointment({ ...appointment, description: e.target.value })}
              />
            </div>

            {/* Status */}
            <div className="form-group">
              <label>Status</label>
              <select
                className="form-control"
                value={appointment.status}
                onChange={(e) => setAppointment({ ...appointment, status: e.target.value })}
              >
                <option value="Upcoming">Upcoming</option>
                <option value="Cancelled">Cancelled</option>
                <option value="Finished">Finished</option>
              </select>
            </div>

            {/* Buttons */}
            <button
              type="button"
              className="btn btn-primary mt-3"
              onClick={handleCreate}
              disabled={loading}
            >
              {loading ? 'Creating...' : 'Create Appointment'}
            </button>
            <button type="button" className="btn btn-secondary mt-3 ml-3" onClick={handleCancel}>
              Cancel
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default AppointmentCreate;
