import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import BackgroundVideo from './Assets/wavesloop.mp4';
import Logo from '../../Shared/Logo.svg';
import Arrow from './Assets/arrow.png';
import './Home.css';
import Carousel from 'react-multi-carousel';
import 'react-multi-carousel/lib/styles.css';
import PlaceholderImage from './Assets/placeholder.png';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const Home = () => {
  const [bestsellers, setBestsellers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();

  // Fetch product details for bestsellers
  useEffect(() => {
    const fetchBestsellers = async () => {
      const bestsellerIds = [8073775972397, 8073606660141, 8073606889517];
      try {
        const requests = bestsellerIds.map((productId) =>
          axios.get(`${environment}/gateway/api/ProxyProduct/${productId}`),
        );
        const responses = await Promise.all(requests);
        setBestsellers(responses.map((response) => response.data));
      } catch (err) {
        setError('Failed to fetch bestsellers.');
      } finally {
        setLoading(false);
      }
    };

    fetchBestsellers();
  }, []);

  const handleViewProduct = (id) => {
    navigate(`/product/${id}`);
  };

  const handleMoreProducts = () => {
    navigate('/products');
  };

  const [email, setEmail] = useState('');
  const [message, setMessage] = useState('');

  const validateEmail = async (email) => {
    const emailRegex = /^[a-zA-Z0-9._-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$/;
    if (emailRegex.test(email)) {
      return true;
    } else {
      return false;
    }
  };

  const handleEmailChange = (event) => {
    setEmail(event.target.value); // Update email state with the input value
  };

  // Function to handle subscription
  const handleSubscribe = async () => {
    const isValidEmail = await validateEmail(email); // Check if email is valid
    if (!isValidEmail) {
      setMessage('Please enter a valid email.');
      return;
    }
    try {
      const response = await fetch(
        `${environment}/gateway/api/ProxyCustomer/subscribe`,
        {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(email),
        },
      );

      if (response.ok) {
        setMessage('Successfully subscribed!');
        setEmail(''); // Clear the input
      } else {
        const error = await response.json();
        setMessage(error.message || 'Subscription failed.');
      }
    } catch (error) {
      setMessage('Error: Unable to subscribe.');
    }
  };

  // Function to handle unsubscription
  const handleUnsubscribe = async () => {
    const isValidEmail = await validateEmail(email); // Check if email is valid
    if (!isValidEmail) {
      setMessage('Please enter a valid email.');
      return;
    }
    try {
      const response = await fetch(
        `${environment}/gateway/api/ProxyCustomer/unsubscribe`,
        {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(email),
        },
      );

      if (response.ok) {
        setMessage('Successfully unsubscribed!');
        setEmail(''); // Clear the input
      } else {
        const error = await response.json();
        setMessage(error.message || 'Unsubscription failed.');
      }
    } catch (error) {
      setMessage('Error: Unable to unsubscribe.');
    }
  };

  const responsive = {
    desktop: { breakpoint: { max: 3000, min: 1024 }, items: 1 },
    tablet: { breakpoint: { max: 1024, min: 464 }, items: 1 },
    mobile: { breakpoint: { max: 464, min: 0 }, items: 1 },
  };

  return (
    <div>
      {/* Background video section */}
      <div className="video-container">
        <video autoPlay loop muted>
          <source src={BackgroundVideo} type="video/mp4" />
          Your browser does not support the video tag.
        </video>
        <div className="logo-container">
          <img src={Logo} alt="Logo" />
          <h1 className="headline">
            Luxury Hair Products
          </h1>
        </div>
        <div
          className="arrow-container"
          onClick={() => {
            const nextSection = document.querySelector(
              '.bestsellers-container',
            );
            nextSection.scrollIntoView({ behavior: 'smooth' });
          }}
        >
          <img src={Arrow} alt="Scroll Down" className="arrow" />
        </div>
      </div>

      {/* Bestsellers Section */}
      <div className="bestsellers-container">
        <h1 className="title-container">Our Bestsellers</h1>
        {loading && <p>Loading...</p>}
        {error && <p className="text-danger">{error}</p>}
        {!loading && !error && bestsellers.length > 0 && (
          <Carousel
            responsive={responsive}
            infinite
            keyBoardControl
            showDots={true}
            containerClass="carousel-container"
            itemClass="carousel-item"
          >
            {bestsellers.map((product) => (
              <div key={product.id} className="bestseller-card">
                <div className="bestseller-content">
                  <img
                    src={PlaceholderImage}
                    alt={product.title}
                    className="bestseller-image"
                  />

                  <div className="bestseller-info">
                    <h1>{product.title}</h1>
                    <p>${product.variants[0]?.price}</p>

                    <div className="divider"></div>
                    <button
                      className="view-product-btn"
                      onClick={() => handleViewProduct(product.id)}
                    >
                      View Product
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </Carousel>
        )}
        <button className="view-all-btn" onClick={handleMoreProducts}>
          View All Products
        </button>
      </div>

      {/* Newsletter Section */}
      <div className="newsletter-container">
        <h1 className="newsletter-title">Subscribe</h1>
        <p className="newsletter-description">
          Unlock exclusive deals and be the first to know about our latest
          drops! <br />
          Sign up for our newsletter!
        </p>
        <div className="newsletter-form">
          <input
            type="email"
            placeholder="Enter your email"
            className="newsletter-input"
            value={email} // Set the input value to the `email` state
            onChange={handleEmailChange}
          />
          <button className="subscribe-btn" onClick={handleSubscribe}>
            Subscribe
          </button>
          <button className="unsubscribe-btn" onClick={handleUnsubscribe}>
            Unsubscribe
          </button>
        </div>
        {message && (
          <p
            style={{
              marginTop: '10px',
              fontSize: '14px',
              color: message.startsWith('Error') ? 'red' : 'green', // Red for errors, green for success
            }}
          >
            {message}
          </p>
        )}
      </div>
    </div>
  );
};

export default Home;
