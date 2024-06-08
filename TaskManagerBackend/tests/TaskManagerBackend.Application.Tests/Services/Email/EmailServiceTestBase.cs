using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerBackend.Application.Services.Email;
using TaskManagerBackend.Tests.Common;

namespace TaskManagerBackend.Application.Tests.Services.Email
{
    public class EmailServiceTestBase : CommonTestBase
    {
        private EmailService CreateEmailService()
        {
            return new EmailService(Mock.Of<ILogger<EmailService>>());
        }

        protected bool ValidateEmailAddressFormat(string emailAddress)
        {
            return CreateEmailService().ValidateEmailAddressFormat(emailAddress);
        }
    }
}