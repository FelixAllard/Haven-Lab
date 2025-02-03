import React, { useState, useEffect } from 'react';
import axios from 'axios';
import { Modal, Button, Table } from 'react-bootstrap';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

const EmailLogs = () => {
  const [emails, setEmails] = useState([]);
  const [selectedEmail, setSelectedEmail] = useState(null);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    axios
      .get(`${environment}/gateway/api/ProxyEmailLog`)
      .then((response) => {
        const mappedEmails = response.data.map((email) => ({
          id: email.id,
          subject: email.emailSubject,
          recipient: email.recipientEmail,
          body: email.emailBody,
          sentDate: new Date(email.sentDate).toLocaleString(), // Format date
        }));
        setEmails(mappedEmails);
      })
      .catch((error) => {
        console.error('Error fetching email logs:', error);
      });
  }, []);

  const handleShow = (email) => {
    setSelectedEmail(email);
    setShowModal(true);
  };

  const handleClose = () => {
    setShowModal(false);
    setSelectedEmail(null);
  };

  return (
    <div className="container mt-4">
      <h2 className="text-light">Email Logs</h2>
      <Table
        striped
        bordered
        hover
        variant="dark"
        className="bg-dark text-light"
      >
        <thead>
          <tr>
            <th>ID</th>
            <th>Subject</th>
            <th>Recipient</th>
            <th>Sent Date</th>
            <th>Actions</th>
          </tr>
        </thead>
        <tbody>
          {emails.map((email) => (
            <tr key={email.id}>
              <td>{email.id}</td>
              <td>{email.subject}</td>
              <td>{email.recipient}</td>
              <td>{email.sentDate}</td>
              <td>
                <Button variant="info" onClick={() => handleShow(email)}>
                  Show
                </Button>
              </td>
            </tr>
          ))}
        </tbody>
      </Table>

      {/* Modal for email body */}
      <Modal show={showModal} onHide={handleClose} size="lg">
        <Modal.Header closeButton className="bg-dark text-light">
          <Modal.Title>Email Details</Modal.Title>
        </Modal.Header>
        <Modal.Body className="bg-dark text-light">
          {selectedEmail && (
            <>
              <p>
                <strong>Subject:</strong> {selectedEmail.subject}
              </p>
              <p>
                <strong>Recipient:</strong> {selectedEmail.recipient}
              </p>
              <p>
                <strong>Sent Date:</strong> {selectedEmail.sentDate}
              </p>
              <iframe
                title="email-body"
                srcDoc={selectedEmail.body}
                style={{
                  width: '100%',
                  height: '400px',
                  border: 'none',
                  background: 'black',
                }}
              ></iframe>
            </>
          )}
        </Modal.Body>
        <Modal.Footer className="bg-dark">
          <Button variant="secondary" onClick={handleClose}>
            Close
          </Button>
        </Modal.Footer>
      </Modal>
    </div>
  );
};

export default EmailLogs;
