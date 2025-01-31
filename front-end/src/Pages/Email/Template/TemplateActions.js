import React from 'react';
import { Button } from 'react-bootstrap';
import { Link } from 'react-router-dom';

const TemplateActions = ({ selectedTemplate, handleDelete, handleViewTemplate }) => {
    return (
        <div>
            <Link className="btn btn-primary me-2" to="/admin/email/template/add">
                Add Template
            </Link>
            <Link
                className="btn btn-warning me-2"
                to={`/modify-template/${selectedTemplate}`}
                disabled={!selectedTemplate}
            >
                Modify Template
            </Link>
            <Button
                variant="info"
                onClick={handleViewTemplate}
                className="me-2"
                disabled={!selectedTemplate}
            >
                View Template
            </Button>
            <Button variant="danger" onClick={handleDelete} disabled={!selectedTemplate}>
                Delete Template
            </Button>
        </div>
    );
};

export default TemplateActions;
