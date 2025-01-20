import React, { useEffect, useState } from 'react';
import axios from 'axios';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useParams, useNavigate } from 'react-router-dom';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;
const AppointmentUpdate = () => {
  const { appointmentId } = useParams();
  const [appointment, setAppointment] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchAppointmentDetails = async () => {
      try {
        const response = await axios.get(`${environment}/gateway/api/ProxyAppointment/${appointmentId}`);
        setAppointment(response.data);
      } catch (err) {
        setError('Failed to fetch appointment details');
      } finally {
        setLoading(false);
      }
    };

    fetchAppointmentDetails();
  }, [appointmentId]);

  const handleUpdate = async () => {
    try {
      const response = await axios.put(`${environment}/gateway/api/ProxyAppointment/${appointmentId}`, appointment);
      console.log(response.status)
      if (response.status === 200) {
        navigate(`/appointments/${appointmentId}`);  // Navigate back to the appointment detail page
      }
    } catch (err) {
      setError('Failed to update appointment');
    }
  };

  // Cancel button handler to go back to the appointment detail page
  const handleCancel = () => {
    navigate(`/appointments/${appointmentId}`); // Navigate back to the appointment detail page
  };

  if (loading) return <div>Loading...</div>;

  if (error) return <div>Error: {error}</div>;

  return (
    <div className="container mt-7">
      <h1>Update Appointment</h1>
      {appointment && (
        <div className="card shadow-sm border-light">
          <div className="card-body">
            <form>
              {/* Appointment Title */}
              <div className="form-group">
                <label>Title</label>
                <input
                  type="text"
                  className="form-control"
                  value={appointment.title || ''}
                  onChange={(e) => setAppointment({ ...appointment, title: e.target.value })}
                />
              </div>

              {/* Customer Name */}
              <div className="form-group">
                <label>Customer Name</label>
                <input
                  type="text"
                  className="form-control"
                  value={appointment.customerName || ''}
                  onChange={(e) => setAppointment({ ...appointment, customerName: e.target.value })}
                />
              </div>

              {/* Customer Email */}
              <div className="form-group">
                <label>Customer Email</label>
                <input
                  type="email"
                  className="form-control"
                  value={appointment.customerEmail || ''}
                  onChange={(e) => setAppointment({ ...appointment, customerEmail: e.target.value })}
                />
              </div>

              {/* Appointment Date */}
              <div className="form-group">
                <label>Date</label>
                <input
                  type="datetime-local"
                  className="form-control"
                  value={appointment.appointmentDate ? new Date(appointment.appointmentDate).toISOString().slice(0, 16) : ''}
                  onChange={(e) => setAppointment({ ...appointment, appointmentDate: e.target.value })}
                />
              </div>

              {/* Description */}
              <div className="form-group">
                <label>Description</label>
                <textarea
                  className="form-control"
                  value={appointment.description || ''}
                  onChange={(e) => setAppointment({ ...appointment, description: e.target.value })}
                />
              </div>

              {/* Status */}
              <div className="form-group">
                <label>Status</label>
                <select
                  className="form-control"
                  value={appointment.status || 'Upcoming'}
                  onChange={(e) => setAppointment({ ...appointment, status: e.target.value })}
                >
                  <option value="Upcoming">Upcoming</option>
                  <option value="Cancelled">Cancelled</option>
                  <option value="Finished">Finished</option>
                </select>
              </div>

              {/* Buttons */}
              <button type="button" className="btn btn-primary mt-3" onClick={handleUpdate}>
                Update Appointment
              </button>
              <button type="button" className="btn btn-secondary mt-3 ml-3" onClick={handleCancel}>
                Cancel
              </button>
            </form>
          </div>
        </div>
      )}
    </div>
  );
};

export default AppointmentUpdate;
