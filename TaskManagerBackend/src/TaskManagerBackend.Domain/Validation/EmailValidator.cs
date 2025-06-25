#region Usings

using System.Net.Mail;
using Microsoft.Extensions.Logging;

#endregion

namespace TaskManagerBackend.Domain.Validation;

public class EmailValidator : IEmailValidator
{
    private readonly ILogger<EmailValidator> _logger;
    
    public EmailValidator(ILogger<EmailValidator> logger)
    {
        _logger = logger;
    }
    
    public bool Validate(string emailAddress)
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