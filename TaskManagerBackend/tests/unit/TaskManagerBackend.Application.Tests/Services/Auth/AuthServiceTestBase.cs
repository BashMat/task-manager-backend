#region Usings

using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Domain.Validation;
using TaskManagerBackend.Tests.Common;

#endregion

namespace TaskManagerBackend.Application.Tests.Services.Auth;

public class AuthServiceTestBase : UnitTestsBase
{
    protected Mock<ICryptographyService> AuthProviderMock { get; private set; }
    protected Mock<IUserRepository> UserRepositoryMock { get; private set; }
    protected Mock<IEmailValidator> EmailServiceMock { get; private set; }
    protected Mock<IDateTimeService> DateTimeServiceMock { get; private set; }

    protected AuthServiceTestBase()
    {
        AuthProviderMock = new Mock<ICryptographyService>();
        UserRepositoryMock = new Mock<IUserRepository>();
        EmailServiceMock = new Mock<IEmailValidator>();
        DateTimeServiceMock = new Mock<IDateTimeService>();
    }

    private AuthService CreateAuthService()
    {
        return new AuthService(AuthProviderMock.Object,
                               UserRepositoryMock.Object,
                               EmailServiceMock.Object,
                               DateTimeServiceMock.Object,
                               Mock.Of<ILogger<AuthService>>());
    }

    protected void SetUpValidateEmailAddressFormat(bool isEmailAddressFormatCorrect = true)
    {
        EmailServiceMock.Setup(o => o.Validate(It.IsAny<string>()))
                        .Returns(isEmailAddressFormatCorrect);
    }
        
    protected void SetUpCheckIfUserExistsByUserNameOrEmail(bool userExists = false)
    {
        UserRepositoryMock.Setup(o => o.CheckIfUserExistsByUserNameOrEmail(It.IsAny<string>(), 
                                                                           It.IsAny<string>()))
                          .ReturnsAsync(userExists);
    }

    protected void SetUpDateTimeService(DateTime utcNow)
    {
        DateTimeServiceMock.Setup(o => o.UtcNow)
                           .Returns(utcNow);
    }

    protected void SetUpCreatePasswordHashAndSalt(byte[] passwordHash, byte[] passwordSalt)
    {
        AuthProviderMock.Setup(o => o.CreatePasswordHashAndSalt(It.IsAny<string>()))
                        .Returns((passwordHash, passwordSalt));
    }

    protected Task<ServiceResponse<UserSignUpResponse>> SignUp(UserSignUpRequest request)
    {
        return CreateAuthService().SignUp(request);
    }
}