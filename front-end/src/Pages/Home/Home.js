import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import axios from 'axios';
import BackgroundVideo from './wavesloop.mp4';
import Logo from '../../Shared/Logo.svg';
import './Home.css'; 

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
                    <h1 className="headline">Luxury Hair Products</h1>
                </div>
            </div>

            {/* Bestsellers Section */}
            <div className="container mt-7">
                <h2 className="mb-4">Bestsellers</h2>
                {loading && <p>Loading...</p>}
                {error && <p className="text-danger">{error}</p>}
                {!loading && !error && bestsellers.length === 0 && (
                    <p className="text-muted">No bestsellers available at the moment.</p>
                )}
                <div className="row">
                    {bestsellers.map((product) => (
                        <div className="col-md-4 mb-4" key={product.id}>
                            <div className="card shadow-sm">
                                <img
                                    src={product.image || 'https://th.bing.com/th/id/R.9c7099669f66da861a45175412ca5877?rik=vPI5%2fbsidkIPFg&pid=ImgRaw&r=0'}
                                    className="card-img-top"
                                    alt={product.title}
                                />
                                <div className="card-body">
                                    <h5 className="card-title">{product.title}</h5>
                                    <p className="card-text">
                                        <strong>Price:</strong> ${product.variants[0]?.price}
                                    </p>
                                    <button
                                        className="btn btn-primary"
                                        onClick={() => handleViewProduct(product.id)}
                                    >
                                        View Product
                                    </button>
                                </div>
                            </div>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default Home;
