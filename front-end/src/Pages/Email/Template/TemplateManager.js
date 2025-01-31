import React, { useState } from 'react';
import { Spinner, Alert, Container } from 'react-bootstrap';
import TemplateList from './TemplateList'; // Import TemplateList
import TemplateActions from './TemplateActions'; // Import TemplateActions
const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const TemplateManager = () => {
  const [templates, setTemplates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedTemplate, setSelectedTemplate] = useState(null);

  const handleDelete = () => {
    if (selectedTemplate) {
      if (
        window.confirm(
          `Are you sure you want to delete template: ${selectedTemplate}?`,
        )
      ) {
        fetch(`${environment}/gateway/api/ProxyTemplate/${selectedTemplate}`, {
          method: 'DELETE',
        })
          .then(() => {
            alert(`Template ${selectedTemplate} deleted successfully.`);
            setTemplates(
              templates.filter((template) => template !== selectedTemplate),
            );
            setSelectedTemplate(null);
          })
          .catch(() => alert('Failed to delete template'));
      }
    }
  };

  const handleViewTemplate = () => {
    if (selectedTemplate) {
      fetch(`${environment}/gateway/api/ProxyTemplate/${selectedTemplate}`)
        .then((response) => response.json())
        .then((data) => {
          alert(`Template details: ${JSON.stringify(data)}`);
        })
        .catch(() => alert('Failed to fetch template details'));
    }
  };

  return (
    <Container
      className="p-4 rounded-3"
      style={{
        backgroundColor: '#000', // Black background
        color: '#fff', // White text for contrast
      }}
    >
      <h2>Template Manager</h2>

      {/* Loading Spinner */}
      {loading && <Spinner animation="border" variant="light" />}

      {/* Error Alert */}
      {error && <Alert variant="danger">{error}</Alert>}

      {/* Template List */}
      <TemplateList
        templates={templates} // Pass templates to TemplateList
        setTemplates={setTemplates}
        setLoading={setLoading}
        setError={setError}
        setSelectedTemplate={setSelectedTemplate}
      />

      {/* Template Actions */}
      <TemplateActions
        selectedTemplate={selectedTemplate}
        handleDelete={handleDelete}
        handleViewTemplate={handleViewTemplate}
      />
    </Container>
  );
};

export default TemplateManager;
