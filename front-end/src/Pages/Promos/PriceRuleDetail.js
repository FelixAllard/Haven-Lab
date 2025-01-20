import React, { useEffect, useState } from 'react';
import { useParams } from 'react-router-dom';
import axios from 'axios';
import { motion } from 'framer-motion';
import { useNavigate } from 'react-router-dom';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const PriceRuleDetail = () => {
    const { priceruleId } = useParams();
    const [priceRule, setPriceRule] = useState(null);
    const [discounts, setDiscounts] = useState({});
    const [setError] = useState(null);
    const navigate = useNavigate();
    const [formData, setFormData] = useState({
        code: ''
    });

    useEffect(() => {
        // Fetch the price rule details by ID
        const fetchPriceRuleDetail = async () => {
            try {
                const response = await axios.get(`${environment}/gateway/api/ProxyPromo/PriceRules/${priceruleId}`);
                console.log("Fetched Price Rule Data:", response.data);
                setPriceRule(response.data);
            } catch (err) {
                console.error('Error fetching price rule details:', err);
            }
        };

        const fetchDiscountCodes = async () => {
            try {
                const response = await axios.get(`${environment}/gateway/api/ProxyPromo/Discounts/${priceruleId}`);
                console.log("Fetched Discount Data:", response.data);
                setDiscounts(response.data.items || []);
            } catch (err) {
                console.error('Error fetching discount details:', err);
            }
        };

        fetchPriceRuleDetail();
        fetchDiscountCodes();
    }, [priceruleId]); // Re-fetch data when the ID changes

    if (!priceRule) {
        return <div className="text-center">Loading price rule details...</div>;
    }


    const handleDelete = async () => {
        try {
            const response = await axios.delete(`${environment}/gateway/api/ProxyPromo/PriceRules/${priceruleId}`);
            if (response.status === 200) {
                navigate(0);
            }
        } catch (err) {
            setError(`Error deleting appointment: ${err.message}`);
        }
    };

    const handleDiscountDelete = async (discountId) => {
        try {
            const response = await axios.delete(`${environment}/gateway/api/ProxyPromo/Discounts/${priceruleId}/${discountId}`);
            if (response.status === 200) {
                navigate(0);
            }
        } catch (err) {
            setError(`Error deleting appointment: ${err.message}`);
        }
    };

    const handleDiscountCreate = async (code) => {
        try {
            const response = await axios.post(`${environment}/gateway/api/ProxyPromo/Discounts/${priceruleId}`, formData);
            if (response.status === 200) {
                navigate(0);
            }
        } catch (err) {
            setError(`Error deleting appointment: ${err.message}`);
        }
    };

    const handleInputChange = (e) => {
        const { name, value } = e.target;
        setFormData({
            ...formData,
            [name]: value
        });
    };
    return (
        <div className="container mt-7 mb-5">
            <h2>Price Rule Details</h2>
            <p><strong>ID:</strong> {priceRule.id}</p>
            <p><strong>Title:</strong> {priceRule.title}</p>
            <p><strong>Value Type:</strong> {new Date(priceRule.value_type).toLocaleString()}</p>
            <p><strong>Value:</strong> {priceRule.value}</p>
            <p><strong>Starts At:</strong> {new Date(priceRule.starts_at).toLocaleString()}</p>
            <p><strong>Ends At:</strong> {priceRule.ends_at ? new Date(priceRule.ends_at).toLocaleString() : 'N/A'}</p>
            <p><strong>Allocation Method:</strong> {priceRule.allocation_method || 'N/A'}</p>
            <p><strong>Target Selection:</strong> {priceRule.target_selection || 'N/A'}</p>
            <p><strong>Target Type:</strong> {priceRule.target_type || 'N/A'}</p>
            <p><strong>Usage Limit:</strong> {priceRule.usage_limit || 'Unlimited'}</p>
            <p><strong>Prerequisite Subtotal:</strong> {priceRule.prerequisite_subtotal_price || 'None'}</p>
            <p><strong>Prerequisite Quantity:</strong> {priceRule.prerequisite_quantity_range || 'None'}</p>
            <p><strong>Created At:</strong> {new Date(priceRule.created_at).toLocaleString()}</p>
            <p><strong>Updated At:</strong> {new Date(priceRule.updated_at).toLocaleString()}</p>

            {/*
            <motion.button
                className="btn btn-secondary mt-3"
                whileHover={{scale: 1.1}}
                transition={{duration: 0.2}}
            >
                <Link
                    to={`/promo/update/pricerules/${priceRule.id}`}
                    style={{color: 'white', textDecoration: 'none'}}
                >
                    Edit
                </Link>
            </motion.button>
            */}

            <motion.button
                className="btn btn-secondary mt-3"
                whileHover={{scale: 1.1}}
                transition={{duration: 0.2}}
                onClick={handleDelete}
            >
                Delete
            </motion.button>

            <h2 className="container mt-4">Discount Codes</h2>

            <div className="row">
                {discounts.length > 0 ? (
                    discounts.map((discount, index) => (
                        <motion.div
                            className="col-md-4 mb-4"
                            key={discount.id}
                            initial={{opacity: 0, y: 50}}
                            animate={{opacity: 1, y: 0}}
                            transition={{duration: 0.5, delay: index * 0.1}}
                        >
                            <div className="card shadow-sm border-light">
                                <div className="card-body">
                                    <p>
                                        <strong>Discount ID:</strong> {discount.id || 'N/A'}
                                    </p>
                                    <p>
                                        <strong>Title:</strong> {discount.code || 'N/A'}
                                    </p>
                                    <p>
                                        <strong>Usage Count:</strong> {discount.usage_count || 'N/A'}
                                    </p>
                                    <p>
                                        <strong>Created At:</strong>{' '}
                                        {discount.created_at
                                            ? new Date(discount.created_at).toLocaleString()
                                            : 'N/A'}
                                    </p>
                                    <button
                                        className="btn btn-primary mt-3"
                                        onClick={() => handleDiscountDelete(discount.id)}
                                    >
                                        Delete
                                    </button>
                                </div>
                            </div>
                        </motion.div>
                    ))
                ) : (
                    <div className="text-center w-100">
                        <p className="text-muted">No discount codes available.</p>
                    </div>
                )}
            </div>

            <div>
                <h2>Add Discount Code</h2>
                <input
                    type="text"
                    name="code"
                    value={formData.code}
                    placeholder="Enter code"
                    onChange={handleInputChange}
                    style={{
                        padding: '10px',
                        fontSize: '16px',
                        borderRadius: '4px',
                        border: '1px solid #007bff',
                        width: '30%',
                        marginBottom: '10px',
                        color: 'black'
                    }}
                />
                <br></br>
                <button className="btn btn-primary mt-3" onClick={() => handleDiscountCreate(formData.code)}>Submit</button>
            </div>
        </div>
    );
};

export default PriceRuleDetail;
