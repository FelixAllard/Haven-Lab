import React, { useState } from 'react';
import httpClient from '../../../AXIOS/AXIOS.js';
import { Modal, Button, Form } from 'react-bootstrap';
import 'bootstrap/dist/css/bootstrap.min.css';
import './AddProductPage.css';

const ProductForm = () => {
  const [formData, setFormData] = useState({
    title: '',
    body_html: '',
    created_at: new Date().toISOString(),
    updated_at: new Date().toISOString(),
    published_at: new Date().toISOString(),
    vendor: '',
    product_type: '',
    handle: '',
    template_suffix: null,
    published_scope: 'global',
    tags: '',
    status: 'active',
    variants: [
      {
        title: 'Default Title',
        price: 19.99,
        inventory_quantity: 5,
        position: 1,
        grams: 0,
        inventory_policy: 'deny',
        fulfillment_service: 'manual',
        requires_shipping: true,
        taxable: true,
        weight_unit: 'kg',
        weight: 0,
        option1: 'Default Title',
        sku: '',
        compare_at_price: null,
        inventory_item_id: null,
        inventory_management: null,
        barcode: null,
        image_id: null,
        metafields: null,
        presentment_prices: null,
        id: null,
        admin_graphql_api_id: null,
      },
    ],
    options: [
      {
        name: 'Title',
        position: 1,
        values: ['Default Title'],
        id: null,
        admin_graphql_api_id: null,
      },
    ],
    images: [],
    metafields: null,
    variant_gids: null,
    id: null,
    admin_graphql_api_id: null,
  });

  const [imageFile, setImageFile] = useState(null);
  const [showSuccess, setShowSuccess] = useState(false);
  const [showError, setShowError] = useState(false);
  const [errorMessage, setErrorMessage] = useState('');
  const [errors, setErrors] = useState({});
  const [frTranslation, setFrTranslation] = useState({
    fr_title: '',
    fr_description: '',
  });

  const handleChange = (e) => {
    const { name, value } = e.target;
    const fieldNames = name.split('.');
    let updatedData = { ...formData };

    // If the field belongs to variants, we handle it differently
    if (fieldNames[0] === 'variants') {
      const variantIndex = parseInt(fieldNames[1], 10); // Get index of the variant
      const field = fieldNames[2]; // Get field name within the variant object

      // Ensure we copy the current variant to preserve immutability
      updatedData.variants = [...updatedData.variants];
      updatedData.variants[variantIndex] = {
        ...updatedData.variants[variantIndex],
        [field]: value, // Update the specific field in the variant
      };
    } else {
      updatedData[name] = value; // For other fields, just update the top-level field
    }

    // Validation for price
    if (name.includes('price')) {
      if (isNaN(value) || Number(value) < 0) {
        setErrors((prevErrors) => ({
          ...prevErrors,
          [name]: 'Price must be a positive number.',
        }));
      } else {
        setErrors((prevErrors) => {
          const newErrors = { ...prevErrors };
          delete newErrors[name];
          return newErrors;
        });
      }
    }

    setFormData(updatedData);
  };

  const handleFrChange = (e) => {
    const { name, value } = e.target;
    setFrTranslation({ ...frTranslation, [name]: value });
  };

  const handleImageChange = (e) => {
    const file = e.target.files[0];
    setImageFile(file);

    // Convert image to base64
    const reader = new FileReader();
    reader.onloadend = () => {
      setImageBase64(reader.result); // store base64 image
    };
    reader.readAsDataURL(file);
  };
  const [imageBase64, setImageBase64] = useState(''); // Base64 encoded image

  const handleSubmit = async (e) => {
    e.preventDefault();
    let formDataToSubmit = { ...formData };

    try {
      if (imageFile) {
        const base64Image = imageBase64.split(',')[1];

        // Send as { ImageData: base64Image }
        const imageResponse = await httpClient.post(
          '/gateway/api/ProxyProduct/upload-image',
          {
            ImageData: base64Image, // Match the backend's property name
          },
        );

        if (imageResponse.status === 200) {
          formDataToSubmit.images = [{ src: imageResponse.data.imageUrl }];
        } else {
          throw new Error('Error uploading image');
        }
      }

      // Proceed with creating the product after image upload
      const response = await httpClient.post(
        `/gateway/api/ProxyProduct`,
        formDataToSubmit,
      );

      if (response.status === 200) {
        const productId = response.data.id;

        // Save French translations in metafields
        const translationData = {
          locale: 'fr',
          title: frTranslation.fr_title,
          description: frTranslation.fr_description,
        };

        await httpClient.post(
          `/gateway/api/ProxyProduct/${productId}/translation`,
          translationData,
        );

        setShowSuccess(true);
        setTimeout(() => {
          window.location.href = '/products';
        }, 2000);
      }
    } catch (error) {
      setShowError(true);
      setErrorMessage(
        error.response?.data?.message || 'An error occurred. Please try again.',
      );
    }
  };

  // Add a new variant
  const addVariant = () => {
    setFormData({
      ...formData,
      variants: [
        ...formData.variants,
        {
          title: 'New Variant',
          price: 0,
          inventory_quantity: 0,
          position: formData.variants.length + 1,
          grams: 0,
          inventory_policy: 'deny',
          fulfillment_service: 'manual',
          requires_shipping: true,
          taxable: true,
          weight_unit: 'kg',
          weight: 0,
          option1: 'New Variant',
          sku: '',
          compare_at_price: null,
          inventory_item_id: null,
          inventory_management: null,
          barcode: null,
          image_id: null,
          metafields: null,
          presentment_prices: null,
          id: null,
          admin_graphql_api_id: null,
        },
      ],
    });
  };

  // Remove a variant by index
  const removeVariant = (index) => {
    const updatedVariants = formData.variants.filter((_, i) => i !== index);
    setFormData({ ...formData, variants: updatedVariants });
  };

  return (
    <div className="container mt-5" style={{ marginBottom: '11%' }}>
      <h2>Create a New Product</h2>

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
          <Form.Label>Description</Form.Label>
          <Form.Control
            as="textarea"
            name="body_html"
            value={formData.body_html}
            onChange={handleChange}
            rows={4}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Title (French)</Form.Label>
          <Form.Control
            type="text"
            name="fr_title"
            value={frTranslation.fr_title}
            onChange={handleFrChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Description (French)</Form.Label>
          <Form.Control
            as="textarea"
            name="fr_description"
            value={frTranslation.fr_description}
            onChange={handleFrChange}
            rows={4}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Product Image</Form.Label>
          <Form.Control
            type="file"
            accept="image/*"
            onChange={handleImageChange}
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Vendor</Form.Label>
          <Form.Control
            type="text"
            name="vendor"
            value={formData.vendor}
            onChange={handleChange}
            required
          />
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Published Scope</Form.Label>
          <Form.Control
            as="select"
            name="published_scope"
            value={formData.published_scope}
            onChange={handleChange}
            required
          >
            <option value="global">Global</option>
            <option value="web">Web</option>
            <option value="storefront">Storefront</option>
          </Form.Control>
        </Form.Group>

        <Form.Group className="mb-3">
          <Form.Label>Status</Form.Label>
          <Form.Control
            as="select"
            name="status"
            value={formData.status}
            onChange={handleChange}
            required
          >
            <option value="active">Active</option>
            <option value="draft">Draft</option>
            <option value="archived">Archived</option>
          </Form.Control>
        </Form.Group>

        {/* Variant Fields */}
        <h4>Variants</h4>
        {formData.variants.map((variant, index) => (
          <div key={index}>
            <Form.Group className="mb-3">
              <Form.Label>Variant Title</Form.Label>
              <Form.Control
                type="text"
                name={`variants.${index}.title`}
                value={variant.title}
                onChange={handleChange}
                required
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Price</Form.Label>
              <Form.Control
                type="number"
                name={`variants.${index}.price`}
                value={variant.price}
                onChange={handleChange}
                step="0.01"
                min="0"
                required
              />
              {errors[`variants.${index}.price`] && (
                <small className="text-danger">
                  {errors[`variants.${index}.price`]}
                </small>
              )}
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>Inventory Quantity</Form.Label>
              <Form.Control
                type="number"
                name={`variants.${index}.inventory_quantity`}
                value={variant.inventory_quantity}
                onChange={handleChange}
                required
              />
            </Form.Group>
            <Form.Group className="mb-3">
              <Form.Label>SKU</Form.Label>
              <Form.Control
                type="text"
                name={`variants.${index}.sku`}
                value={variant.sku}
                onChange={handleChange}
              />
            </Form.Group>
            {/* Add other variant fields similarly */}
            {formData.variants.length > 1 && (
              <Button variant="danger" onClick={() => removeVariant(index)}>
                Remove Variant
              </Button>
            )}
          </div>
        ))}

        <Button variant="secondary" onClick={addVariant}>
          Add Variant
        </Button>

        <Button variant="primary" type="submit">
          Submit
        </Button>
      </Form>

      {/* Success Modal */}
      <Modal show={showSuccess} onHide={() => setShowSuccess(false)}>
        <Modal.Header closeButton>
          <Modal.Title>Success!</Modal.Title>
        </Modal.Header>
        <Modal.Body>Your product has been created successfully!</Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowSuccess(false)}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>

      {/* Error Modal */}
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

export default ProductForm;
