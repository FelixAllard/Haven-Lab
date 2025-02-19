import React, { useState } from 'react';
import { Spinner, Alert, Container, Modal, Button } from 'react-bootstrap';
import TemplateList from './TemplateList';
import TemplateActions from './TemplateActions';
import { Link } from 'react-router-dom';
import httpClient from '../../../../AXIOS/AXIOS';

const TemplateManager = () => {
  const [templates, setTemplates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedTemplate, setSelectedTemplate] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [templateDetails, setTemplateDetails] = useState(null);

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
          setTemplateDetails(response.data);
          setShowModal(true);
        })
        .catch(() => alert('Failed to fetch template details'));
    }
  };

  const getEmailContent = (htmlContent) => {
    // Use a regular expression to extract the <style> tags and the content inside them
    const styleMatch = htmlContent.match(/<style[^>]*>([\s\S]*?)<\/style>/g);

    // Extract the styles and the content
    const styles = styleMatch ? styleMatch.join('') : '';
    const contentWithoutStyles = htmlContent.replace(
      /<style[^>]*>[\s\S]*?<\/style>/g,
      '',
    );

    // Wrap the content with its styles
    return `
      <style>${styles}</style>
      ${contentWithoutStyles}
    `;
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

      {/* Template Details Modal */}
      <Modal show={showModal} onHide={() => setShowModal(false)} centered>
        <Modal.Header closeButton>
          <Modal.Title style={{ color: 'black' }}>
            {templateDetails?.templateName || 'Template Details'}
          </Modal.Title>
        </Modal.Header>
        <Modal.Body className="text-bg-dark">
          {templateDetails?.emailTemplate?.htmlFormat ? (
            <div
              dangerouslySetInnerHTML={{
                __html: getEmailContent(
                  templateDetails.emailTemplate.htmlFormat,
                ),
              }}
            />
          ) : (
            <p>No HTML content available.</p>
          )}
        </Modal.Body>
        <Modal.Footer>
          <Button variant="secondary" onClick={() => setShowModal(false)}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </Container>
  );
};

export default TemplateManager;
