import React, { useEffect } from 'react';
const environment = process.env.REACT_APP_API_GATEWAY_HOST;
const TemplateList = ({
  templates,
  setTemplates,
  setLoading,
  setError,
  setSelectedTemplate,
}) => {
  useEffect(() => {
    fetch(`${environment}/gateway/api/ProxyTemplate/names`)
      .then((response) => response.json())
      .then((data) => {
        setTemplates(data);
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
