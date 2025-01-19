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


const Home = () => {
    const [bestsellers, setBestsellers] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    const navigate = useNavigate();

    // Fetch product details for bestsellers
    useEffect(() => {
        const fetchBestsellers = async () => {
            const bestsellerIds = [
                8073898131501,
                8073775972397,
                8073518088237,
            ];
            try {
                const requests = bestsellerIds.map((productId) =>
                    axios.get(`http://localhost:5158/gateway/api/ProxyProduct/${productId}`)
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
                    <source src={BackgroundVideo} type="video/mp4"/>
                    Your browser does not support the video tag.
                </video>
                <div className="logo-container">
                    <img src={Logo} alt="Logo"/>
                    <h1 className="headline">Luxury Hair Products</h1>
                </div>
                <div
                    className="arrow-container"
                    onClick={() => {
                        const nextSection = document.querySelector('.bestsellers-container');
                        nextSection.scrollIntoView({behavior: 'smooth'});
                    }}
                >
                    <img src={Arrow} alt="Scroll Down" className="arrow"/>
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
                                        <p>
                                            ${product.variants[0]?.price}
                                        </p>

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
                    Unlock exclusive deals and be the first to know about our latest drops! <br/>
                    Sign up for our newsletter!
                </p>
                <div className="newsletter-form">
                    <input
                        type="email"
                        placeholder="Enter your email"
                        className="newsletter-input"
                    />
                    <button className="subscribe-btn">Subscribe</button>
                </div>
            </div>
        </div>
    );
};

export default Home;
