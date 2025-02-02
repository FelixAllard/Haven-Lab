import React, { useEffect, useState } from 'react';
import { Navigate, useLocation } from 'react-router-dom'; 
import { useAuth } from './AuthentificationContext';

const ProtectedRoute = ({ children }) => {
  const { verifyToken, loading } = useAuth();
  const [isTokenValid, setIsTokenValid] = useState(null); // null means verifying
  const location = useLocation(); // Get the current location

  useEffect(() => {
    let isMounted = true; // Add a flag to track if the component is mounted

    const verifyAuthToken = async () => {
      const isValid = await verifyToken();
      if (isMounted) {
        setIsTokenValid(isValid);
      }
    };

    verifyAuthToken();

    return () => {
      isMounted = false; // Cleanup function to set isMounted to false
    };
  }, [verifyToken, location]);

  if (loading || isTokenValid === null) {
    return <div>Loading...</div>;
  }

  return isTokenValid ? children : <Navigate to="/admin/login" replace />;
};

export default ProtectedRoute;