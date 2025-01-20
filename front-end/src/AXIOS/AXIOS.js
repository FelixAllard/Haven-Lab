import axios from 'axios';

// Create an instance of Axios with default settings
const httpClient = axios.create({
  baseURL: 'https://api.example.com', // Default base URL
  headers: {
    'Content-Type': 'application/json',
  },
});

export const setAuthToken = (token) => {
  if (token) {
    // Set the Authorization header with Bearer token
    httpClient.defaults.headers['Authorization'] = `Bearer ${token}`;
  } else {
    // Remove the Authorization header if no token
    delete httpClient.defaults.headers['Authorization'];
  }
};

export default httpClient;
