import React, {
  createContext,
  useContext,
  useState,
  useCallback,
} from 'react';
import httpClient from './AXIOS';

const AuthContext = createContext();

export const AuthProvider = ({ children }) => {
  const [authToken, setAuthTokenState] = useState(
    localStorage.getItem('authToken') || null,
  );
  const [username, setUsername] = useState(
    localStorage.getItem('username') || null,
  );
  const [loading, setLoading] = useState(true);

  const login = async (username, password) => {
    try {
      setLoading(true);
      const response = await httpClient.post(
        `/gateway/api/ProxyAuth/login`,
        {
          username,
          password,
        },
      );

      const token = response.data.token;
      setAuthTokenState(token);
      localStorage.setItem('authToken', token);
      localStorage.setItem('username', username);
      setUsername(username);
      setLoading(false);

      return token;
    } catch (error) {
      setLoading(false);
      console.error('Login failed:', error);
      throw error;
    }
  };



  const logout = async () => {
    try {
      setLoading(true);
      const storedUsername = username || localStorage.getItem('username');
      await httpClient.post(
        `/gateway/api/proxyAuth/logout`,
        storedUsername,
        {
          headers: {
            'Content-Type': 'application/json-patch+json',
            accept: '*/*',
          },
        },
      );

      setAuthTokenState(null);
      localStorage.removeItem('authToken');
      localStorage.removeItem('username');
      setUsername(null);
      setLoading(false);
    } catch (error) {
      setLoading(false);
      console.error('Logout failed:', error);
      throw error;
    }
  };

  const verifyToken = useCallback(async () => {
    try {
      setLoading(true);
      const response = await httpClient.post(
        `/gateway/api/proxyAuth/verify-token`,
        authToken,
        {
          headers: { 'Content-Type': 'application/json' },
        },
      );

      setLoading(false);
      return response.status === 200;
    } catch (error) {
      setLoading(false);
      console.error('Token verification failed:', error);
      setAuthTokenState(null);
      localStorage.removeItem('authToken');
      localStorage.removeItem('username');
      setUsername(null);
      return false;
    }
  }, [authToken]);

  return (
    <AuthContext.Provider
      value={{ authToken, username, login, logout, verifyToken, loading }}
    >
      {children}
    </AuthContext.Provider>
  );
};

export const useAuth = () => {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
};
