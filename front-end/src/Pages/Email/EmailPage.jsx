import EmailSendPage from './EmailSendPage';
import TemplateManager from './Template/TemplateManager';

export const EmailPage = () => {
  return (
    <>
      <div className="container text-center">
        <div className="row h-100 d-flex align-items-center justify-content-center">
          <div className="col-md-4">
            <TemplateManager />
          </div>

          {/* Use col-md-4 for medium screens */}
          <div className="col-md-8">
            <EmailSendPage />
          </div>
        </div>
      </div>
    </>
  );
};
