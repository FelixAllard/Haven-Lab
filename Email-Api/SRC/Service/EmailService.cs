using Email_Api.Exceptions;
using Email_Api.Model;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace Email_Api.Service;

public class EmailService : IEmailService
{
    private readonly ISmtpConnection _smtpConnection;
    private readonly ITemplateManager _templateManager;

    public EmailService(
        ISmtpConnection smtpConnection, 
        ITemplateManager templateManager
        )
    {
        _smtpConnection = smtpConnection;
        _templateManager = templateManager;
    }
    public OperationResult SendEmail(DirectEmailModel model)
    {
        DirectEmailModel directEmailModel = model;
        
        if (String.IsNullOrWhiteSpace(directEmailModel.EmailToSendTo))
            throw new BadEmailModel("Email To Send To is null or whitespace. EMAIL IS REQUIRED");
        if(!EmailUtils.CheckIfEmailIsValid(directEmailModel.EmailToSendTo))
            throw new BadEmailModel("Email To Send To Not Valid");
        if(String.IsNullOrWhiteSpace(directEmailModel.EmailTitle))
            throw new BadEmailModel("Email Title is null or whitespace");
        if (String.IsNullOrWhiteSpace(directEmailModel.TemplateName))
            directEmailModel.TemplateName = "Default";
        EmailTemplate? emailTemplate;
        try
        {
            emailTemplate = _templateManager.GetTemplate(directEmailModel.TemplateName);
        }
        catch (KeyNotFoundException e)
        {
            Console.WriteLine(e);
            throw;
        }
        string builtEmail;
        try
        {
            string header = "";
            string body = "";
            string footer = "";
            string correspondentName = "";
            string sender = "";
            if (!String.IsNullOrWhiteSpace(directEmailModel.Header))
                header = directEmailModel.Header;
            if (!String.IsNullOrWhiteSpace(directEmailModel.Body))
                body = directEmailModel.Body;
            if (!String.IsNullOrWhiteSpace(directEmailModel.Footer))
                footer = directEmailModel.Footer;
            if (!String.IsNullOrWhiteSpace(directEmailModel.CorrespondantName))
                correspondentName = directEmailModel.CorrespondantName;
            if (!String.IsNullOrWhiteSpace(directEmailModel.SenderName))
                sender = directEmailModel.SenderName;
            builtEmail = emailTemplate.BuildEmail(
                header,
                body,
                footer,
                correspondentName,
                sender
            );
        }
        catch (TemplateRequiredFieldNotSet e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        
        _smtpConnection.SendEmailAsync(
            directEmailModel.EmailToSendTo, 
            directEmailModel.EmailTitle, 
            builtEmail);
        // Run the email sending task in a separate thread
        return new OperationResult
        {
            Success = true, 
            Message = builtEmail, 
            Result = "Successfully sent email!"
        };
    }
}