import React, { useState, useEffect } from "react";
import { useScroll, motion, useTransform, useSpring } from "framer-motion";
import { FaFacebook, FaTwitter, FaInstagram, FaLinkedin } from "react-icons/fa";
import { useNavigate } from "react-router-dom";
import "./Footer.css"; // For custom styles

const Footer = () => {
    const [showFooter, setShowFooter] = useState(false);
    const { scrollYProgress } = useScroll(); // Track the vertical scroll progress
    const [isBottom, setIsBottom] = useState(false);
    const navigate = useNavigate();

    // Motion value for scaling the footer's panel
    const footerScale = useTransform(scrollYProgress, [0, 0.8], [0, 1]);
    const fadeInDelay = useSpring(useTransform(scrollYProgress, [0.85, 1], [0, 1]));

    useEffect(() => {
        const checkBottom = () => {
            const scrollPosition = window.scrollY + window.innerHeight;
            const documentHeight = document.documentElement.scrollHeight;
            if (scrollPosition >= documentHeight - 10) { // Check if we are near the bottom
                setIsBottom(true);
            } else {
                setIsBottom(false);
            }
        };

        window.addEventListener("scroll", checkBottom);
        checkBottom(); // Check immediately on mount

        return () => window.removeEventListener("scroll", checkBottom);
    }, []);

    useEffect(() => {
        if (isBottom) {
            setShowFooter(true);
        } else {
            setShowFooter(false);
        }
    }, [isBottom]);

    const handleLoginClick = () => {
        navigate("/admin/login"); // Redirect to the login page
    };

    return (
        <motion.div
            className={`footer-container ${showFooter ? "show-footer" : ""}`}
            style={{ scaleY: footerScale }}
            initial={{ opacity: 0 }}
            animate={{ opacity: 1 }}
            transition={{
                duration: 0.5, // Quick appearance
                ease: "easeOut",
            }}
        >
            <motion.div className="footer-content container">
                <div className="row">
                    <motion.div
                        className="col-md-4"
                        style={{ opacity: fadeInDelay }}
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{
                            delay: 0.2, // Quick appearance with delay for sequential reveal
                            duration: 0.3,
                            ease: "easeInOut",
                        }}
                    >
                        <h5>About Us</h5>
                        <p>Some information about Haven Lab</p>
                    </motion.div>
                    <motion.div
                        className="col-md-4"
                        style={{ opacity: fadeInDelay }}
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{
                            delay: 0.4, // Slightly delayed
                            duration: 0.3,
                            ease: "easeInOut",
                        }}
                    >
                        <h5>Contact</h5>
                        <p>Phone: (123) 456-7890</p>
                        <p>Email: info@example.com</p>
                    </motion.div>
                    <motion.div
                        className="col-md-4"
                        style={{ opacity: fadeInDelay }}
                        initial={{ opacity: 0 }}
                        animate={{ opacity: 1 }}
                        transition={{
                            delay: 0.6, // Further delay for the last section
                            duration: 0.3,
                            ease: "easeInOut",
                        }}
                    >
                        <h5>Follow Us</h5>
                        <div className="social-media-icons">
                            <a href="https://facebook.com" target="_blank" rel="noopener noreferrer">
                                <FaFacebook />
                            </a>
                            <a href="https://twitter.com" target="_blank" rel="noopener noreferrer">
                                <FaTwitter />
                            </a>
                            <a href="https://instagram.com" target="_blank" rel="noopener noreferrer">
                                <FaInstagram />
                            </a>
                            <a href="https://linkedin.com" target="_blank" rel="noopener noreferrer">
                                <FaLinkedin />
                            </a>
                        </div>
                    </motion.div>
                </div>
                {/* Add "Owner Login" button */}
                <div className="row mt-3">
                    <div className="col text-center">
                        <button
                            className="btn btn-primary owner-login-button"
                            onClick={handleLoginClick}
                        >
                            Owner Login
                        </button>
                    </div>
                </div>
            </motion.div>
        </motion.div>
    );
};

export default Footer;
