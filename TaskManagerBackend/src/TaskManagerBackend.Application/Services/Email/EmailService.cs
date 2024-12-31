#region Usings

using System.Net.Mail;

#endregion

namespace TaskManagerBackend.Application.Services.Email;

public class EmailService : IEmailService
{
    private readonly ILogger<EmailService> _logger;
    
    public EmailService(ILogger<EmailService> logger)
    {
        _logger = logger;
    }
    
    public bool ValidateEmailAddressFormat(string emailAddress)
    {
        try
        {
            return new MailAddress(emailAddress).Address == emailAddress;
        }
        catch (Exception e)
        {
            _logger.LogWarning(e, "User specified incorrect email address");
            return false;
        }
    }
}