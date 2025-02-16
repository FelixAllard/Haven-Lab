import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import 'bootstrap/dist/css/bootstrap.min.css';
import { useAuth } from '../../../AXIOS/AuthentificationContext';
import Cookies from 'js-cookie';
import { useTranslation } from 'react-i18next';

const OwnerLogin = () => {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const navigate = useNavigate();
  const { login } = useAuth();
  const { i18n } = useTranslation();

  const handleLogin = async (e) => {
    e.preventDefault();
    try {
      const token = await login(username, password);

      if (token) {
        Cookies.set('language', "en", { expires: 365 });
        i18n.changeLanguage("en");
        navigate('/');
      }
    } catch (error) {
      console.error('Login failed:', error);

      if (error.response) {
        setError(
          error.response.data?.message ||
            'An error occurred. Please try again.',
        );
      } else {
        setError('An error occurred while logging in. Please try again.');
      }
    }
  };

  return (
    <div className="d-flex justify-content-center align-items-center vh-100">
      <div className="card shadow-lg" style={{ width: '400px' }}>
        <div className="card-body">
          <h2 className="card-title text-center mb-4">Owner Login</h2>
          {error && (
            <div className="alert alert-danger text-center" role="alert">
              {error}
            </div>
          )}
          <form onSubmit={handleLogin}>
            <div className="mb-3">
              <label htmlFor="username" className="form-label">
                Username
              </label>
              <input
                type="text"
                className="form-control"
                id="username"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </div>
            <div className="mb-3">
              <label htmlFor="password" className="form-label">
                Password
              </label>
              <input
                type="password"
                className="form-control"
                id="password"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
            </div>
            <button type="submit" className="btn btn-primary w-100">
              Login
            </button>
          </form>
        </div>
      </div>
    </div>
  );
};

export default OwnerLogin;
