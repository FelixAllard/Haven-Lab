import React, { createContext, useContext, useState, useEffect } from 'react';
import { setAuthToken } from './AXIOS'; // The HTTP client service

// Create context
const AuthContext = createContext();

// Create a provider component
export const AuthProvider = ({ children }) => {
    const [authToken, setAuthTokenState] = useState(localStorage.getItem('authToken') || null);

    useEffect(() => {
        // Whenever the token changes, update the axios authorization header
        setAuthToken(authToken);
    }, [authToken]);

    // Method to log in (example: saving token)
    const login = (token) => {
        setAuthTokenState(token);
        localStorage.setItem('authToken', token);
    };

    // Method to log out
    const logout = () => {
        setAuthTokenState(null);
        localStorage.removeItem('authToken');
    };

    return (
        <AuthContext.Provider value={{ authToken, login, logout }}>
            {children}
        </AuthContext.Provider>
    );
};

// Custom hook to access the auth context
export const useAuth = () => {
    const context = useContext(AuthContext);
    if (!context) {
        throw new Error('useAuth must be used within an AuthProvider');
    }
    return context;
};
