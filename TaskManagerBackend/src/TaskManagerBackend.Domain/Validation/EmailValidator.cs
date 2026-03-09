#region Usings

using System.Net.Mail;
using Microsoft.Extensions.Logging;

#endregion

namespace TaskManagerBackend.Domain.Validation;

public class EmailValidator(ILogger<EmailValidator> logger) : IEmailValidator
{
    public bool Validate(string emailAddress)
    {
        try
        {
            return new MailAddress(emailAddress).Address == emailAddress;
        }
        catch (Exception e)
        {
            logger.LogWarning(e, "User specified incorrect email address");
            return false;
        }
    }
}