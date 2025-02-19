import React, { useState, useEffect } from 'react';
import { Modal, Button, Form } from 'react-bootstrap';
import { useParams } from 'react-router-dom'; // Import useParams for route params
import 'bootstrap/dist/css/bootstrap.min.css';
import './UpdatePriceRule.css';
import httpClient from '../../../AXIOS/AXIOS';

const UpdatePriceRule = () => {
  const { priceruleId } = useParams(); // Retrieve id from URL params

  const [formData, setFormData] = useState({
    title: '',
    target_type: 'line_item',
    target_selection: 'entitled',
    allocation_method: 'each',
    value_type: 'percentage',
    value: -10,
    once_per_customer: true,
    usage_limit: 100,
    customer_selection: 'all',
    starts_at: new Date().toISOString(),
    ends_at: new Date().toISOString(),
    entitled_product_ids: [],
    prerequisite_product_ids: [],
    prerequisite_to_entitlement_quantity_ratio: {
      prerequisite_quantity: 2,
      entitled_quantity: 1,
    },
  });

  const [showSuccess, setShowSuccess] = useState(false);
  const [showError, setShowError] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');

  // Fetch price rule data if id exists
  useEffect(() => {
    if (priceruleId) {
      const fetchPriceRule = async () => {
        try {
          console.log('FETCH');
          const response = await httpClient.get(
            `/gateway/api/ProxyPromo/PriceRules/${priceruleId}`,
          );
          console.log('Fetched Price Rule Data:', response.data);
          setFormData(response.data);
        } catch (err) {
          console.error('Error fetching price rule details:', err);
        }
      };
      fetchPriceRule();
    }
  }, [priceruleId]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    const updatedData = { ...formData, [name]: value };
    setFormData(updatedData);
  };

  const handleJSONChange = (e) => {
    const { name, value } = e.target;
    try {
      const parsedValue = JSON.parse(value);
      setFormData((prevState) => ({
        ...prevState,
        [name]: parsedValue,
      }));
    } catch (error) {
      console.error('Invalid JSON input');
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      if (!formData.id) {
        throw new Error('Price Rule ID is required for updating.');
      }

      const response = await httpClient.put(
        `/gateway/api/ProxyPromo/PriceRules/${formData.id}`,
        formData,
      );
      if (response.status === 200) {
        setShowSuccess(true);
        setTimeout(() => {
          window.location.href = '/admin/promo/pricerules'; // Redirect to list of price rules
        }, 2000);
      }
    } catch (error) {
      setShowError(true);
      setErrorMessage(
        error.response?.data?.message || 'An error occurred. Please try again.',
      );
    }
  };

  return (
    <div className="container mt-7" style={{ marginBottom: '11%' }}>
      <h2>Edit Price Rule</h2>

      <Form onSubmit={handleSubmit}>
        <Form.Group className="mb-3">
          <Form.Label>Title</Form.Label>
          <Form.Control
            type="text"
            name="title"
            value={formData.title}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Value Type</Form.Label>
          <Form.Control
            as="select"
            name="value_type"
            value={formData.value_type}
            onChange={handleChange}
            required
          >
            <option value="percentage">Percentage</option>
            <option value="fixed_amount">Fixed Amount</option>
          </Form.Control>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Value</Form.Label>
          <Form.Control
            type="number"
            name="value"
            value={formData.value}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Starts At</Form.Label>
          <Form.Control
            type="datetime-local"
            name="starts_at"
            value={formData.starts_at}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Ends At</Form.Label>
          <Form.Control
            type="datetime-local"
            name="ends_at"
            value={formData.ends_at}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Entitled Product IDs</Form.Label>
          <Form.Control
            as="textarea"
            name="entitled_product_ids"
            value={JSON.stringify(formData.entitled_product_ids)}
            onChange={handleJSONChange}
            rows={2}
            placeholder='["8073775972397"]'
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Prerequisite Product IDs</Form.Label>
          <Form.Control
            as="textarea"
            name="prerequisite_product_ids"
            value={JSON.stringify(formData.prerequisite_product_ids)}
            onChange={handleJSONChange}
            rows={2}
            placeholder='["8073775972397"]'
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Prerequisite Quantity</Form.Label>
          <Form.Control
            type="number"
            name="prerequisite_quantity"
            value={
              formData.prerequisite_to_entitlement_quantity_ratio
                .prerequisite_quantity
            }
            onChange={(e) =>
              setFormData({
                ...formData,
                prerequisite_to_entitlement_quantity_ratio: {
                  ...formData.prerequisite_to_entitlement_quantity_ratio,
                  prerequisite_quantity: e.target.value,
                },
              })
            }
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Entitled Quantity</Form.Label>
          <Form.Control
            type="number"
            name="entitled_quantity"
            value={
              formData.prerequisite_to_entitlement_quantity_ratio
                .entitled_quantity
            }
            onChange={(e) =>
              setFormData({
                ...formData,
                prerequisite_to_entitlement_quantity_ratio: {
                  ...formData.prerequisite_to_entitlement_quantity_ratio,
                  entitled_quantity: e.target.value,
                },
              })
            }
            required
          />
        </Form.Group>

        <Button variant="success" type="submit" className="mt-3">
          Save Changes
        </Button>
      </Form>

      {/* Success/Error Modals */}
      <Modal show={showSuccess} onHide={() => setShowSuccess(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Success</Modal.Title>
        </Modal.Header>
        <Modal.Body>Your price rule has been updated successfully!</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowSuccess(false)}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>

      <Modal show={showError} onHide={() => setShowError(false)}>
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
    </div>
  );
};

export default UpdatePriceRule;
