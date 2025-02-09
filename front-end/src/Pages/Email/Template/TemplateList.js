import React, { useEffect } from 'react';
import httpClient from '../../../AXIOS/AXIOS';

const TemplateList = ({
  templates,
  setTemplates,
  setLoading,
  setError,
  setSelectedTemplate,
}) => {
  useEffect(() => {
    httpClient
      .get('/gateway/api/ProxyTemplate/names')
      .then((response) => {
        setTemplates(response.data);
        setLoading(false);
      })
      .catch(() => {
        setError('Failed to load templates');
        setLoading(false);
      });
  }, [setTemplates, setLoading, setError]);

  return (
    <div className="mb-3">
      <label>Select a Template</label>
      <select
        className="form-select"
        onChange={(e) => setSelectedTemplate(e.target.value)}
      >
        <option value="">--Select Template--</option>
        {/* Map through templates and create an option for each */}
        {templates.map((template) => (
          <option key={template} value={template}>
            {template}
          </option>
        ))}
      </select>
    </div>
  );
};

export default TemplateList;
