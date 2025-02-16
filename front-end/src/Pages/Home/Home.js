import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import BackgroundVideo from './Assets/wavesloop.mp4';
import Logo from '../../Shared/Logo.svg';
import Arrow from './Assets/arrow.png';
import './Home.css';
import Carousel from 'react-multi-carousel';
import 'react-multi-carousel/lib/styles.css';
import httpClient from '../../AXIOS/AXIOS';
import "../../Languages/i18n.js";
import { useTranslation } from 'react-i18next';

const Home = () => {
  const [bestsellers, setBestsellers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const navigate = useNavigate();
  const {t} = useTranslation('home');

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
          <h1 className="headline">{t("Tagline")}</h1>
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
        <h1 className="title-container">{t("Bestsellers")}</h1>
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
                    src={
                      product.images[0]?.src ||
                      require('../../Shared/imageNotFound.jpg')
                    }
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
                      {t("View Product")}
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </Carousel>
        )}
        <button className="view-all-btn" onClick={handleMoreProducts}>
          {t("View All")}
        </button>
      </div>

      {/* Newsletter Section */}
      <div className="newsletter-container">
        <h1 className="newsletter-title">{t("Subscribe")}</h1>
        <p className="newsletter-description">
          {t("Subscribe P1")} <br />
          Sign up for our newsletter!
        </p>
        <div className="newsletter-form">
          <input
            type="email"
            placeholder={t("EmailPlaceholder")}
            className="newsletter-input"
            value={email} // Set the input value to the `email` state
            onChange={handleEmailChange}
          />
          <button className="subscribe-btn" onClick={handleSubscribe}>
            {t("Subscribe")}
          </button>
          <button className="unsubscribe-btn" onClick={handleUnsubscribe}>
            {t("Unsubscribe")}
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
