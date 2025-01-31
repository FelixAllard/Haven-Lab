import React, { useState, useRef, useEffect } from 'react';
import axios from 'axios';
import { toast } from 'react-toastify';
import 'react-toastify/dist/ReactToastify.css';
import { Link, useParams } from 'react-router-dom';

const environment = process.env.REACT_APP_API_GATEWAY_HOST;

// EditTemplatePage Component
const EditTemplatePage = () => {
  const { templatename } = useParams(); // Extract templatename from the URL
  const [templateName, setTemplateName] = useState('');
  const [htmlFormat, setHtmlFormat] = useState('');
  const [isLoading, setIsLoading] = useState(false);
  const [submitSuccess, setSubmitSuccess] = useState(null); // Add a state to track success/failure

  const iframeRef = useRef(null);

  useEffect(() => {
    // Fetch the existing template data when the component mounts
    const fetchTemplate = async () => {
      try {
        setIsLoading(true);
        const response = await axios.get(
          `${environment}/gateway/api/ProxyTemplate/${templatename}`,
          {
            headers: {
              accept: '*/*',
            },
          },
        );

        if (response.status === 200) {
          setTemplateName(response.data.templateName);
          setHtmlFormat(response.data.emailTemplate.htmlFormat);
          updatePreview(response.data.emailTemplate.htmlFormat);
        } else {
          toast.error('Failed to fetch the template data.');
        }
      } catch (error) {
        toast.error('Error fetching the template data.');
      } finally {
        setIsLoading(false);
      }
    };

    fetchTemplate();
  }, [templatename]); // Fetch data whenever templatename changes

  const handleTemplateNameChange = (e) => setTemplateName(e.target.value);
  const handleHtmlFormatChange = (e) => {
    setHtmlFormat(e.target.value);
    updatePreview(e.target.value);
  };

  const updatePreview = (htmlContent) => {
    if (iframeRef.current) {
      const iframeDocument = iframeRef.current.contentDocument;
      iframeDocument.open();
      iframeDocument.write(htmlContent);
      iframeDocument.close();
    }
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const payload = {
      templateName: templateName,
      emailTemplate: {
        htmlFormat: htmlFormat,
      },
    };

    try {
      setIsLoading(true);
      const response = await axios.put(
        `${environment}/gateway/api/ProxyTemplate/${templatename}`,
        payload,
        {
          headers: {
            accept: '*/*',
            'Content-Type': 'application/json-patch+json',
          },
        },
      );

      if (response.status === 200) {
        toast.success('Email template updated successfully!');
        setSubmitSuccess(true); // Set success state
      } else {
        toast.error('Failed to update the email template.');
        setSubmitSuccess(false); // Set failure state
      }
    } catch (error) {
      toast.error('Error updating the email template.');
      setSubmitSuccess(false); // Set failure state
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="container my-5">
      <div className="row justify-content-center">
        <div className="col-md-8 col-lg-6">
          <div className="card shadow-sm p-4">
            <h1 className="text-center mb-4">Edit Email Template</h1>
            <form onSubmit={handleSubmit}>
              <div className="mb-3">
                <label htmlFor="templateName" className="form-label">
                  Template Name:
                </label>
                <input
                  type="text"
                  id="templateName"
                  value={templateName}
                  onChange={handleTemplateNameChange}
                  className="form-control"
                  required
                />
              </div>
              <div className="">
                <label htmlFor="htmlFormat" className="form-label">
                  HTML Format:
                </label>
                <textarea
                  id="htmlFormat"
                  value={htmlFormat}
                  onChange={handleHtmlFormatChange}
                  className="form-control"
                  rows="6"
                  required
                />
              </div>

              {/* Legend Section */}
              <div className="mt-3">
                <h5>Legend:</h5>
                <ul className="list-group">
                  <li className="list-group-item">
                    %%EMAIL_HEADER%% - Placeholder for the header.
                  </li>
                  <li className="list-group-item">
                    %%EMAIL_BODY%% - Placeholder for the body of the email.
                  </li>
                  <li className="list-group-item">
                    %%EMAIL_FOOTER%% - Placeholder for the footer.
                  </li>
                  <li className="list-group-item">
                    %%EMAIL_NAME%% - Placeholder for the recipient's name.
                  </li>
                  <li className="list-group-item">
                    %%EMAIL_SENDER%% - Placeholder for the sender's name.
                  </li>
                </ul>
              </div>
              <div className="mb-3">
                <h2>Preview:</h2>
                <iframe
                  ref={iframeRef}
                  style={{
                    width: '100%',
                    height: '300px',
                    border: '1px solid #ccc',
                    backgroundColor: '#f4f4f4',
                  }}
                  title="Preview"
                />
              </div>

              {/* Add a status message based on the form submission result */}
              {submitSuccess !== null && (
                <div
                  className={`mb-3 alert ${submitSuccess ? 'alert-success' : 'alert-danger'}`}
                >
                  {submitSuccess
                    ? 'Template updated successfully!'
                    : 'Failed to update the template.'}
                </div>
              )}

              <button
                type="submit"
                className="btn btn-primary w-100"
                disabled={isLoading}
              >
                {isLoading ? 'Updating...' : 'Update Template'}
              </button>
            </form>
            <div className="mt-3 text-center">
              <Link className="btn btn-warning" to="/admin/email/send">
                Go back
              </Link>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
};

export default EditTemplatePage;
