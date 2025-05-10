#region Usings

using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerBackend.Domain.Validators;
using TaskManagerBackend.Tests.Common;

#endregion

namespace TaskManagerBackend.Domain.Tests.Email;

public class EmailValidatorTestBase : UnitTestsBase
{
    private EmailValidator CreateEmailValidator()
    {
        return new EmailValidator(Mock.Of<ILogger<EmailValidator>>());
    }

    protected bool ValidateEmailAddressFormat(string emailAddress)
    {
        return CreateEmailValidator().ValidateEmailAddressFormat(emailAddress);
    }
}