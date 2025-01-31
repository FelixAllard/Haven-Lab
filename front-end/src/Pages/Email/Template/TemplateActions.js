import React from 'react';
import { Button, ButtonGroup } from 'react-bootstrap';
import { Link } from 'react-router-dom';

const TemplateActions = ({
  selectedTemplate,
  handleDelete,
  handleViewTemplate,
}) => {
  return (
    <ButtonGroup aria-label="Template Actions" className="mb-3">
      {/* Add Template Button */}
      <Link className="btn btn-primary" to="/admin/email/template/add">
        <i className="fas fa-plus me-2"></i>Add Template
      </Link>

      {/* Modify Template Button */}
      <Link
        className="btn btn-warning"
        to={`/admin/email/template/modify/${selectedTemplate}`}
        disabled={!selectedTemplate}
      >
        <i className="fas fa-edit me-2"></i>Modify Template
      </Link>

      {/* View Template Button */}
      <Button
        variant="info"
        onClick={handleViewTemplate}
        disabled={!selectedTemplate}
      >
        <i className="fas fa-eye me-2"></i>View Template
      </Button>

      {/* Delete Template Button */}
      <Button
        variant="danger"
        onClick={handleDelete}
        disabled={!selectedTemplate}
      >
        <i className="fas fa-trash me-2"></i>Delete Template
      </Button>
    </ButtonGroup>
  );
};

export default TemplateActions;
