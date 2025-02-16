import React, { useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './NavBar.css';
import { motion } from 'motion/react';
import { Modal, Button } from 'react-bootstrap';
import { useAuth } from '../AXIOS/AuthentificationContext'; // Import useAuth hook
import LanguageToggle from '../Languages/LanguageToggle'
import Logo from '../Shared/Logo.svg';
import "../Languages/i18n.js";
import { useTranslation } from 'react-i18next';

const Navbar = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);
  const [showError, setShowError] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [modalClosed, setModalClosed] = useState(false);
  const [logoutTimer, setLogoutTimer] = useState(null);
  const {t} = useTranslation('navbar');

  const { authToken, logout } = useAuth(); // Use useAuth hook to get authToken and logout
  const navigate = useNavigate();

  const isLoggedIn = !!authToken; // Check if the user is logged in

  const handleToggle = () => {
    setIsOpen(!isOpen);
  };

  const handleLogout = () => {
    try {
      logout(); // Call logout from context
      setShowSuccess(true);

      const timer = setTimeout(() => {
        if (!modalClosed) {
          setShowSuccess(false);
          navigate('/'); // Redirect to home after successful logout
        }
      }, 3000);

      setLogoutTimer(timer);
    } catch (error) {
      setErrorMessage('An error occurred while logging out.');
      setShowError(true);
    }
  };

  const handleModalClose = () => {
    setModalClosed(true);
    setShowSuccess(false);
    if (logoutTimer) {
      clearTimeout(logoutTimer);
    }
    navigate('/');
  };

  return (
    <>
      <nav className="navbar navbar-dark fixed-top">
        <div className="container-fluid d-flex align-items-center justify-content-between">
          {/* Center Section - Logo */}
          <div className="position-absolute top-50 start-50 translate-middle">
            <Link to="/">
              <img src={Logo} alt="Haven Lab Logo" className="navbar-logo" />
            </Link>
          </div>

          {/* Left Section */}

            <div className="d-flex align-items-center gap-3">

              {!isLoggedIn && (
                <LanguageToggle />
              )}

              {isLoggedIn && (
                <div>
                  <span className="navbar-brand">Owner</span>
                  <button className="btn btn-outline-light" onClick={handleLogout}>
                    Logout
                  </button>
                </div>
              )}

            </div>

          <button
            className="navbar-toggler"
            type="button"
            onClick={handleToggle}
            data-bs-toggle="offcanvas"
            data-bs-target="#offcanvasNavbar"
            aria-controls="offcanvasNavbar"
            aria-label="Toggle navigation"
          >
            <span className="navbar-toggler-icon"></span>
          </button>
        </div>

        <div
          className="offcanvas offcanvas-end text-bg-dark"
          tabIndex="-1"
          id="offcanvasNavbar"
          aria-labelledby="offcanvasNavbarLabel"
        >
          <div className="offcanvas-header">
            <button
              type="button"
              className="btn-close"
              data-bs-dismiss="offcanvas"
              aria-label="Close"
            ></button>
          </div>
          <div className="offcanvas-body">
            <ul className="navbar-nav justify-content-end flex-grow-1 pe-3">
              <motion.li
                key={isOpen ? 'home-open' : 'home-closed'}
                className="nav-item"
                initial={{ opacity: 0, scale: 0.1 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.1 }}
                transition={{ delay: 0.5, duration: 0.7 }}
              >
                <Link className="nav-link" aria-current="page" to="/">
                  {t("Home")}
                </Link>
              </motion.li>

              <motion.li
                key={isOpen ? 'products-open' : 'products-closed'}
                className="nav-item"
                initial={{ opacity: 0, scale: 0.1 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.1 }}
                transition={{ delay: 0.5, duration: 0.7 }}
              >
                <Link className="nav-link" aria-current="page" to="/products">
                  {t("Products")}
                </Link>
              </motion.li>

              <motion.li
                key={isOpen ? 'cart-open' : 'cart-closed'}
                className="nav-item"
                initial={{ opacity: 0, scale: 0.1 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.1 }}
                transition={{ delay: 0.5, duration: 0.7 }}
              >
                <Link className="nav-link" aria-current="page" to="/cart">
                  {t("Cart")}
                </Link>
              </motion.li>

              {isLoggedIn && (
                <motion.li
                  key={isOpen ? 'orders-open' : 'orders-closed'}
                  className="nav-item"
                  initial={{ opacity: 0, scale: 0.1 }}
                  animate={{ opacity: 1, scale: 1 }}
                  exit={{ opacity: 0, scale: 0.1 }}
                  transition={{ delay: 0.5, duration: 0.7 }}
                >
                  <Link className="nav-link" aria-current="page" to="/orders">
                    Orders
                  </Link>
                </motion.li>
              )}

              {isLoggedIn && (
                <motion.li
                  key={isOpen ? 'Emails-open' : 'Emails-closed'}
                  className="nav-item"
                  initial={{ opacity: 0, scale: 0.1 }}
                  animate={{ opacity: 1, scale: 1 }}
                  exit={{ opacity: 0, scale: 0.1 }}
                  transition={{ delay: 0.5, duration: 0.7 }}
                >
                  <Link
                    className="nav-link"
                    aria-current="page"
                    to="/admin/email/send"
                  >
                    Emails
                  </Link>
                </motion.li>
              )}

              {isLoggedIn && (
                <motion.li
                  key={isOpen ? 'appointments-open' : 'appointments-closed'}
                  className="nav-item"
                  initial={{ opacity: 0, scale: 0.1 }}
                  animate={{ opacity: 1, scale: 1 }}
                  exit={{ opacity: 0, scale: 0.1 }}
                  transition={{ delay: 0.5, duration: 0.7 }}
                >
                  <Link
                    className="nav-link"
                    aria-current="page"
                    to="/appointments"
                  >
                    Appointments
                  </Link>
                </motion.li>
              )}

              {isLoggedIn && (
                <motion.li
                  key={isOpen ? 'promos-open' : 'promos-closed'}
                  className="nav-item"
                  initial={{ opacity: 0, scale: 0.1 }}
                  animate={{ opacity: 1, scale: 1 }}
                  exit={{ opacity: 0, scale: 0.1 }}
                  transition={{ delay: 0.5, duration: 0.7 }}
                >
                  <Link
                    className="nav-link"
                    aria-current="page"
                    to="/promo/pricerules"
                  >
                    Promos
                  </Link>
                </motion.li>
              )}

              <motion.li
                key={isOpen ? 'about-open' : 'about-closed'}
                className="nav-item"
                initial={{ opacity: 0, scale: 0.1 }}
                animate={{ opacity: 1, scale: 1 }}
                exit={{ opacity: 0, scale: 0.1 }}
                transition={{ delay: 0.5, duration: 0.7 }}
              >
                <Link className="nav-link" to="/aboutus">
                  {t("About Us")}
                </Link>
              </motion.li>
            </ul>
          </div>
        </div>
      </nav>

      {/* Success Modal */}
      <Modal show={showSuccess} onHide={handleModalClose} centered animation>
        <Modal.Header closeButton>
          <Modal.Title>Success</Modal.Title>
        </Modal.Header>
        <Modal.Body>You have successfully logged out.</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={handleModalClose}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>

      {/* Error Modal */}
      <Modal
        show={showError}
        onHide={() => setShowError(false)}
        centered
        animation
      >
        <Modal.Header closeButton>
          <Modal.Title>Error</Modal.Title>
        </Modal.Header>
        <Modal.Body>{errorMessage}</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowError(false)}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </>
  );
};

export default Navbar;
