import React, { useState } from 'react';
import { Link } from 'react-router-dom';
import './NavBar.css'; // Ensure your CSS is imported
import { motion } from 'motion/react'; // Ensure you're using framer-motion correctly

const Navbar = () => {
    const [isOpen, setIsOpen] = useState(false);

    const handleToggle = () => {
        setIsOpen(!isOpen);
    };

    return (
        <nav className="navbar navbar-dark bg-dark fixed-top">
            <div className="container-fluid">
                <Link className="navbar-brand" aria-current="page" to="/">Haven Lab</Link>
                <button className="navbar-toggler" type="button" onClick={handleToggle} data-bs-toggle="offcanvas"
                        data-bs-target="#offcanvasNavbar" aria-controls="offcanvasNavbar"
                        aria-label="Toggle navigation">
                    <span className="navbar-toggler-icon"></span>
                </button>
                <div className="offcanvas offcanvas-end text-bg-dark" tabIndex="-1" id="offcanvasNavbar"
                     aria-labelledby="offcanvasNavbarLabel">
                    <div className="offcanvas-header">
                        <h5 className="offcanvas-title h1" id="offcanvasNavbarLabel">Welcome to Haven Lab</h5>
                        <button type="button" className="btn-close" data-bs-dismiss="offcanvas"
                                aria-label="Close"></button>
                    </div>
                    <div className="offcanvas-body">
                        <ul className="navbar-nav justify-content-end flex-grow-1 pe-3">
                            <motion.li
                                key={isOpen ? "home-open" : "home-closed"} // Use key to reset animation each time it opens
                                className="nav-item"
                                initial={{opacity: 0, scale: 0.1}}  // Initial state: invisible and small
                                animate={{opacity: 1, scale: 1}} // Animate to full opacity and size each time it opens
                                exit={{opacity: 0, scale: 0.1}} // Ensure it resets when closing
                                transition={{delay: 0.5, duration: 0.7}} // Animation settings
                            >
                                <Link className="nav-link" aria-current="page" to="/">Home</Link>
                            </motion.li>
                            <motion.li
                                key={isOpen ? "products-open" : "products-closed"} // Use key to reset animation each time it opens
                                className="nav-item"
                                initial={{opacity: 0, scale: 0.1}}  // Initial state: invisible and small
                                animate={{opacity: 1, scale: 1}} // Animate to full opacity and size each time it opens
                                exit={{opacity: 0, scale: 0.1}} // Ensure it resets when closing
                                transition={{delay: 0.5, duration: 0.7}} // Animation settings
                            >
                                <Link className="nav-link" aria-current="page" to="/products">Products</Link>
                            </motion.li>
                            <motion.li
                                key={isOpen ? "orders-open" : "orders-closed"} // Use key to reset animation each time it opens
                                className="nav-item"
                                initial={{opacity: 0, scale: 0.1}}  // Initial state: invisible and small
                                animate={{opacity: 1, scale: 1}} // Animate to full opacity and size each time it opens
                                exit={{opacity: 0, scale: 0.1}} // Ensure it resets when closing
                                transition={{delay: 0.5, duration: 0.7}} // Animation settings
                            >
                                <Link className="nav-link" aria-current="page" to="/orders">Orders</Link>
                            </motion.li>
                            <motion.li
                                key={isOpen ? "about-open" : "about-closed"} // Use key to reset animation each time it opens
                                className="nav-item"
                                initial={{opacity: 0, scale: 0.1}}  // Initial state: invisible and small
                                animate={{opacity: 1, scale: 1}} // Animate to full opacity and size each time it opens
                                exit={{opacity: 0, scale: 0.1}} // Ensure it resets when closing
                                transition={{delay: 0.5, duration: 0.7}} // Animation settings
                            >
                                <Link className="nav-link" to="/aboutus">About Us</Link>
                            </motion.li>
                            {/* Add more nav links as needed with similar motion */}
                        </ul>
                    </div>
                </div>
            </div>
        </nav>
    );
};

export default Navbar;
