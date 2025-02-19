import React, { useState, useEffect, useRef } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './NavBar.css';
import { motion } from 'motion/react';
import { Modal, Button } from 'react-bootstrap';
import { useAuth } from '../AXIOS/AuthentificationContext';
import LanguageToggle from '../Languages/LanguageToggle';
import Logo from '../Shared/Logo.svg';
import '../Languages/i18n.js';
import { useTranslation } from 'react-i18next';
import HoverScaleWrapper from '../Shared/HoverScaleWrapper';

const Navbar = () => {
  const [isOpen, setIsOpen] = useState(false);
  const [showSuccess, setShowSuccess] = useState(false);
  const [showError, setShowError] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [modalClosed, setModalClosed] = useState(false);
  const [logoutTimer, setLogoutTimer] = useState(null);
  const [showToggleButton, setShowToggleButton] = useState(false);
  const { t } = useTranslation('navbar');


  const { authToken, logout } = useAuth();
  const navigate = useNavigate();

  const isLoggedIn = !!authToken;

  const navbarRef = useRef(null);
  const navItemsRef = useRef(null);

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

  useEffect(() => {
    const checkOrientation = () => {
      const isVertical = window.innerHeight > window.innerWidth;
      setShowToggleButton(isVertical); // Show button only if vertical screen
    };

    checkOrientation(); // Initial check
    window.addEventListener('resize', checkOrientation); // Re-check on resize
    return () => window.removeEventListener('resize', checkOrientation);
  }, []);

  return (
      <>
        <nav className="navbar navbar-dark fixed-top" ref={navbarRef}>
          <div className="container-fluid d-flex align-items-center justify-content-between">
            {/* Left Section - Logo */}
            <div className="d-flex align-items-center gap-3">
              <HoverScaleWrapper>
                <Link to="/">
                  <img src={Logo} alt="Haven Lab Logo" className="navbar-logo" style={{ maxWidth: '150px' }} />
                </Link>
              </HoverScaleWrapper>

              {!isLoggedIn && <LanguageToggle />}

              {isLoggedIn && (
                  <div>
                    <span className="navbar-brand">Owner</span>
                    <button className="btn btn-outline-light" onClick={handleLogout}>
                      Logout
                    </button>
                  </div>
              )}
            </div>

            {/* Right Section - Navigation Items */}
            <div className="d-flex align-items-center gap-3" ref={navItemsRef}>
              {!showToggleButton && (
                  <>
                    <HoverScaleWrapper>
                      <Link className="nav-link fs-3" to="/">
                        {t('Home')}
                      </Link>
                    </HoverScaleWrapper>
                    <HoverScaleWrapper>
                      <Link className="nav-link fs-3" to="/products">
                        {t('Products')}
                      </Link>
                    </HoverScaleWrapper>
                    <HoverScaleWrapper>
                      <Link className="nav-link fs-3" to="/cart">
                        {t('Cart')}
                      </Link>
                    </HoverScaleWrapper>

                    {isLoggedIn && (
                        <>
                          <HoverScaleWrapper>
                            <Link className="nav-link fs-3" to="/orders">
                              Orders
                            </Link>
                          </HoverScaleWrapper>
                          <HoverScaleWrapper>
                            <Link className="nav-link fs-3" to="/admin/email/send">
                              Emails
                            </Link>
                          </HoverScaleWrapper>
                          <HoverScaleWrapper>
                            <Link className="nav-link fs-3" to="/appointments">
                              Appointments
                            </Link>
                          </HoverScaleWrapper>
                          <HoverScaleWrapper>
                            <Link className="nav-link fs-3" to="/promo/pricerules">
                              Promos
                            </Link>
                          </HoverScaleWrapper>

                        </>
                    )}
                    <HoverScaleWrapper>
                      <Link className="nav-link fs-3" to="/aboutus">
                        {t('About Us')}
                      </Link>
                    </HoverScaleWrapper>

                  </>
              )}
            </div>

            {showToggleButton && (
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
            )}
          </div>

          {/* Offcanvas Menu */}
          {showToggleButton && (
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
                    <HoverScaleWrapper>
                      <li className="nav-item ">
                        <Link className="nav-link fs-1" to="/">
                          {t('Home')}
                        </Link>
                      </li>
                    </HoverScaleWrapper>

                    <HoverScaleWrapper>
                      <li className="nav-item">
                        <Link className="nav-link fs-1" to="/products">
                          {t('Products')}
                        </Link>
                      </li>
                    </HoverScaleWrapper>
                    <HoverScaleWrapper>
                      <li className="nav-item">
                        <Link className="nav-link fs-1" to="/cart">
                          {t('Cart')}
                        </Link>
                      </li>
                    </HoverScaleWrapper>

              {isLoggedIn && (
                <motion.li
                  key={isOpen ? 'orders-open' : 'orders-closed'}
                  className="nav-item"
                  initial={{ opacity: 0, scale: 0.1 }}
                  animate={{ opacity: 1, scale: 1 }}
                  exit={{ opacity: 0, scale: 0.1 }}
                  transition={{ delay: 0.5, duration: 0.7 }}
                >
                  <Link
                    className="nav-link"
                    aria-current="page"
                    to="/admin/orders"
                  >
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
                    to="/admin/appointments"
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
                    to="/admin/promo/pricerules"
                  >
                    Promos
                  </Link>
                </motion.li>
              )}
                    {isLoggedIn && (
                        <>
                          <HoverScaleWrapper>
                            <li className="nav-item">
                              <Link className="nav-link fs-1" to="/orders">
                                Orders
                              </Link>
                            </li>
                          </HoverScaleWrapper>
                          <HoverScaleWrapper>
                            <li className="nav-item">
                              <Link className="nav-link fs-1" to="/admin/email/send">
                                Emails
                              </Link>
                            </li>
                          </HoverScaleWrapper>
                          <HoverScaleWrapper>
                            <li className="nav-item">
                              <Link className="nav-link fs-1" to="/appointments">
                                Appointments
                              </Link>
                            </li>
                          </HoverScaleWrapper>
                          <HoverScaleWrapper>
                            <li className="nav-item">
                              <Link className="nav-link fs-1" to="/promo/pricerules">
                                Promos
                              </Link>
                            </li>
                          </HoverScaleWrapper>

                        </>
                    )}
                    <HoverScaleWrapper>
                      <li className="nav-item">
                        <Link className="nav-link fs-1" to="/aboutus">
                          {t('About Us')}
                        </Link>
                      </li>
                    </HoverScaleWrapper>

                  </ul>
                </div>
              </div>
          )}
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
        <Modal show={showError} onHide={() => setShowError(false)} centered animation>
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
