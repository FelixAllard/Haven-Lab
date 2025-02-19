import React, { useEffect, useState } from 'react';
import { Form, Button, Alert } from 'react-bootstrap';
import httpClient from '../../../AXIOS/AXIOS';

const EmailForm = () => {
  const [emailToSendTo, setEmailToSendTo] = useState('');
  const [emailTitle, setEmailTitle] = useState('');
  const [templateName, setTemplateName] = useState('');
  const [header, setHeader] = useState('');
  const [body, setBody] = useState('');
  const [footer, setFooter] = useState('');
  const [correspondantName, setCorrespondantName] = useState('');
  const [senderName, setSenderName] = useState('');
  const [message, setMessage] = useState('');
  const [messageType, setMessageType] = useState('');

  const [templates, setTemplates] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const handleSubmit = async (e) => {
    e.preventDefault();

    const directEmailModel = {
      emailToSendTo,
      emailTitle,
      templateName,
      header,
      body,
      footer,
      correspondantName,
      senderName,
    };

    try {
      const response = await httpClient.post(
        '/gateway/api/ProxyEmailApi/sendwithformat',
        directEmailModel,
      );

      if (response.ok) {
        setMessage('Email sent successfully!');
        setMessageType('success');
      } else {
        const errorData = await response.json();
        setMessage(`Error: ${errorData.message || 'Something went wrong'}`);
        setMessageType('error');
      }
    } catch (error) {
      setMessage(`Error: ${error.message}`);
      setMessageType('error');
    }
  };
  useEffect(() => {
    const fetchTemplates = async () => {
      try {
        const response = await httpClient.get(
          '/gateway/api/ProxyTemplate/names',
        );
        setTemplates(response.data);
      } catch (err) {
        setError('Failed to load templates');
      } finally {
        setLoading(false);
      }
    };

    fetchTemplates();
  }, []);

  return (
    <div className="container mt-7 mb-5">
      <div className="card p-4">
        <h2 className="text-center mb-4">Send Email with Template</h2>
        <Form onSubmit={handleSubmit}>
          <Form.Group controlId="emailToSendTo">
            <Form.Label>Email To</Form.Label>
            <Form.Control
              type="email"
              value={emailToSendTo}
              onChange={(e) => setEmailToSendTo(e.target.value)}
              required
            />
          </Form.Group>

          <Form.Group controlId="emailTitle">
            <Form.Label>Email Title</Form.Label>
            <Form.Control
              type="text"
              value={emailTitle}
              onChange={(e) => setEmailTitle(e.target.value)}
              required
            />
          </Form.Group>

          <Form.Group controlId="templateName">
            <Form.Label>Template Name</Form.Label>
            {loading ? (
              <div>Loading...</div>
            ) : error ? (
              <div>{error}</div>
            ) : (
              <Form.Control
                as="select"
                value={templateName}
                onChange={(e) => setTemplateName(e.target.value)}
                required
              >
                <option value="">Select a Template</option>
                {templates.map((template, index) => (
                  <option key={index} value={template}>
                    {template}
                  </option>
                ))}
              </Form.Control>
            )}
          </Form.Group>

          <Form.Group controlId="header">
            <Form.Label>Header</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              value={header}
              onChange={(e) => setHeader(e.target.value)}
            />
          </Form.Group>

          <Form.Group controlId="body">
            <Form.Label>Body</Form.Label>
            <Form.Control
              as="textarea"
              rows={5}
              value={body}
              onChange={(e) => setBody(e.target.value)}
            />
          </Form.Group>

          <Form.Group controlId="footer">
            <Form.Label>Footer</Form.Label>
            <Form.Control
              as="textarea"
              rows={3}
              value={footer}
              onChange={(e) => setFooter(e.target.value)}
            />
          </Form.Group>

          <Form.Group controlId="correspondantName">
            <Form.Label>Correspondant Name</Form.Label>
            <Form.Control
              type="text"
              value={correspondantName}
              onChange={(e) => setCorrespondantName(e.target.value)}
              required
            />
          </Form.Group>

          <Form.Group controlId="senderName">
            <Form.Label>Sender Name</Form.Label>
            <Form.Control
              type="text"
              value={senderName}
              onChange={(e) => setSenderName(e.target.value)}
              required
            />
          </Form.Group>

          <Button variant="primary" type="submit" className="w-100 mt-3">
            Send Email
          </Button>
        </Form>

        {message && (
          <Alert
            variant={messageType === 'success' ? 'success' : 'danger'}
            className="mt-4"
          >
            {message}
          </Alert>
        )}
      </div>
    </div>
  );
};

export default EmailForm;
