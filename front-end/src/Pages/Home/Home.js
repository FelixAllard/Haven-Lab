import React, { useEffect, useState } from 'react';
import { Link, useNavigate } from 'react-router-dom';
import BackgroundVideo from './Assets/wavesloop.mp4';
import Logo from '../../Shared/Logo.svg';
import Arrow from './Assets/arrow.png';
import './Home.css';
import { motion } from 'motion/react';
import httpClient from '../../AXIOS/AXIOS';
import '../../Languages/i18n.js';
import { useTranslation } from 'react-i18next';
import HoverScaleWrapper from "../../Shared/HoverScaleWrapper";

const Home = () => {
  const [bestsellers, setBestsellers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();
  const { t } = useTranslation('home');

  //Fetch products containing "bestseller" in its tags
  useEffect(() => {
    const fetchProducts = async () => {
      try {
        const response = await httpClient.get('/gateway/api/ProxyProduct');

        console.log('API Response:', response.data);

        // Ensure data.items is an array before filtering
        if (Array.isArray(response.data.items)) {
          const bestsellerProducts = response.data.items.filter(
            (product) => product.tags && product.tags.includes('bestseller'),
          );
          setBestsellers(bestsellerProducts);
        } else {
          console.error('Unexpected API response format', response.data);
        }
      } catch (error) {
        console.error('Error fetching products:', error);
        setError(error);
      } finally {
        setLoading(false);
      }
    };

    fetchProducts();
  }, []);

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
    setEmail(event.target.value);
  };

  const handleSubscribe = async () => {
    const isValidEmail = await validateEmail(email);
    if (!isValidEmail) {
      setMessage('Invalid email, please enter a valid email.');
      return;
    }

    try {
      const response = await httpClient.post(
        `/gateway/api/ProxyCustomer/subscribe`,
        email,
      );

      if (response.status === 200) {
        setMessage('Successfully subscribed!');
        setEmail('');
      } else {
        setMessage(response.data.message || 'Subscription failed.');
      }
    } catch (error) {
      if (error.response) {
        setMessage(error.response.data.message || 'Subscription failed.');
      } else if (error.request) {
        setMessage('Error: No response from the server.');
      } else {
        setMessage('Error: Unable to subscribe.');
      }
    }
  };

  const handleUnsubscribe = async () => {
    const isValidEmail = await validateEmail(email);
    if (!isValidEmail) {
      setMessage('Invalid email, please enter a valid email.');
      return;
    }

    try {
      const response = await httpClient.post(
        `/gateway/api/ProxyCustomer/unsubscribe`,
        email,
      );

      if (response.status === 200) {
        setMessage('Successfully unsubscribed!');
        setEmail('');
      } else {
        setMessage(response.data.message || 'Unsubscription failed.');
      }
    } catch (error) {
      if (error.response) {
        setMessage(error.response.data.message || 'Unsubscription failed.');
      } else if (error.request) {
        setMessage('Error: No response from the server.');
      } else {
        setMessage('Error: Unable to unsubscribe.');
      }
    }
  };

  const isErrorMessage = (msg) => {
    const errorKeywords = [
      'failed',
      'error',
      'unable',
      'unsuccessful',
      'invalid',
      'no response',
    ];
    return errorKeywords.some((keyword) => msg.toLowerCase().includes(keyword));
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
          <h1 className="headline">{t('Tagline')}</h1>
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
        <h1 className="title-container mb-5">{t('Bestsellers')}</h1>
        {loading && <p>Loading...</p>}
        {error && <p className="text-danger">{error}</p>}

        {!loading && !error && bestsellers.length > 0 && (
          <div className="container">
            <div className="row">
              {bestsellers.map((product, index) => (
                <motion.div
                  className="col-md-4 mb-4"
                  key={product.id}
                  initial={{ opacity: 0, y: 50 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.5, delay: index * 0.1 }}
                >
                  <div className="card product-card">
                    <img
                      src={
                        product.images[0]?.src ||
                        require('../../Shared/imageNotFound.jpg')
                      }
                      className="card-img-top"
                      alt={product.title}
                    />
                    <div className="card-body">
                      <h5 className="card-title">{product.title}</h5>
                      <p className="price">${product.variants[0]?.price}</p>
                      <Link
                        to={`/product/${product.id}`}
                        className="btn btn-secondary btn-block"
                      >
                        {t('View Product')}
                      </Link>
                    </div>
                  </div>
                </motion.div>
              ))}
            </div>
          </div>
        )}
        <HoverScaleWrapper>
          <button className="view-all-btn" onClick={handleMoreProducts}>
            {t('View All')}
          </button>
        </HoverScaleWrapper>

      </div>

      {/* Newsletter Section */}
      <div className="newsletter-container">
      <h1 className="newsletter-title">{t('Subscribe')}</h1>
        <p className="newsletter-description">
          {t('Subscribe P1')} <br />
          Sign up for our newsletter!
        </p>
        <div className="newsletter-form">
          <input
            type="email"
            placeholder={t('EmailPlaceholder')}
            className="newsletter-input"
            value={email} // Set the input value to the `email` state
            onChange={handleEmailChange}
          />
          <button className="subscribe-btn" onClick={handleSubscribe}>
            {t('Subscribe')}
          </button>
          <button className="unsubscribe-btn" onClick={handleUnsubscribe}>
            {t('Unsubscribe')}
          </button>
        </div>
        {message && (
          <p
            className={
              isErrorMessage(message) ? 'error-message' : 'success-message'
            }
          >
            {message}
          </p>
        )}
      </div>
    </div>
  );
};

export default Home;
