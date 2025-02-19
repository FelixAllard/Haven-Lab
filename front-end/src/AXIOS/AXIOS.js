import axios from 'axios';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

// Create an instance of Axios with default settings
const httpClient = axios.create({
  baseURL: environment,
  headers: {
    'Content-Type': 'application/json',
  },
});

var token = localStorage.getItem('authToken');
if (token) {
  httpClient.defaults.headers['Authorization'] = `Bearer ${token}`;
} else {
  // Remove the Authorization header if no token
  delete httpClient.defaults.headers['Authorization'];
}

export default httpClient;
