using System.Net;
using System.Net.Mail;
using System.Text.RegularExpressions;
using Email_Api.Model;
using MailKit.Net.Smtp;
using SmtpClient = System.Net.Mail.SmtpClient;

namespace Email_Api;

public static class EmailUtils
{
    //public static List<EmailTemplate> EmailTemplates = new List<EmailTemplate>();
    //Can be disabled mostly for testing
    //public static bool enableLogging = true;
    
    
    /// <summary>
    /// Test if an email is valid with an expression validated by online sources
    /// </summary>
    /// <param name="email">The email to verify</param>
    /// <returns>True if the email is valid and false if the email is null, empty or invalid</returns>
    public static bool CheckIfEmailIsValid(string email)
    {
        if (string.IsNullOrWhiteSpace(email))
        {
            return false;
        }
        // Regular expression for validating an email
        string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(email, emailPattern);
    }
}