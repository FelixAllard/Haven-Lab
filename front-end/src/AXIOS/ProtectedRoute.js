import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from './AuthentificationContext';

const ProtectedRoute = ({ children }) => {
    const { authToken } = useAuth();

    return authToken ? children : <Navigate to="/admin/login" replace />;
};

export default ProtectedRoute;
