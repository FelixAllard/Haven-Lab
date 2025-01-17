using Email_Api.Model;
using Microsoft.AspNetCore.Mvc;

namespace Email_Api.Service;

public interface IEmailService
{

    public OperationResult SendEmail(DirectEmailModel model);
}