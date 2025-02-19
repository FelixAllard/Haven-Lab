import React, { useState } from 'react';
import { Spinner, Alert, Container } from 'react-bootstrap';
import TemplateList from './TemplateList'; // Import TemplateList
import TemplateActions from './TemplateActions';
import { Link } from 'react-router-dom'; // Import TemplateActions
import httpClient from '../../../../AXIOS/AXIOS';

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
        httpClient
          .delete(`/gateway/api/ProxyTemplate/${selectedTemplate}`)
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
      httpClient
        .get(`/gateway/api/ProxyTemplate/${selectedTemplate}`)
        .then((response) => {
          alert(`Template details: ${JSON.stringify(response.data)}`);
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
      <h1>Logs</h1>
      <Link className="btn btn-primary" to="/admin/email/sent">
        <i className="fas fa-plus me-2"></i>View Sent Emails
      </Link>
    </Container>
  );
};

export default TemplateManager;
