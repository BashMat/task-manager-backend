#region Usings

using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerBackend.Application.Services.Auth;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Domain.Validators;
using TaskManagerBackend.Dto.User;
using TaskManagerBackend.Tests.Common;

#endregion

namespace TaskManagerBackend.Application.Tests.Services.Auth;

public class AuthServiceTestBase : CommonTestBase
{
    private const string DefaultToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZXhwIjoxNzEwNjA2MTUzfQ.Qr4baSoGgjHXUkHQ4ILRJTGBXUA4d_l7fQzV_dLV899n-K2O5hAelYl1zMM3cVEMeAk-4NwRlsJZpfb-dPMnlA";

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
        EmailServiceMock.Setup(o => o.ValidateEmailAddressFormat(It.IsAny<string>()))
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

    protected Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto request)
    {
        return CreateAuthService().SignUp(request);
    }
        
    protected void SetUpUserRepositoryMock(int? userId = null, byte[]? passwordHash = null, byte[]? passwordSalt = null)
    {
        UserPasswordData? result = null;
            
        if (userId != null && passwordHash != null && passwordSalt != null)
        {
            result = new UserPasswordData
                     {
                         Id = (int)userId,
                         PasswordHash = passwordHash,
                         PasswordSalt = passwordSalt
                     };
        }
            
        UserRepositoryMock.Setup(o => o.GetUserPasswordData(It.IsAny<string>()))
                          .ReturnsAsync(result);
    }

    protected void SetUpAuthProviderMock(bool isPasswordHashCorrect = true, string token = DefaultToken)
    {
        AuthProviderMock.Setup(o => o.VerifyPasswordHash(It.IsAny<string>(), 
                                                         It.IsAny<byte[]>(), 
                                                         It.IsAny<byte[]>()))
                        .Returns(isPasswordHashCorrect);
        AuthProviderMock.Setup(o => o.CreateToken(It.IsAny<int>()))
                        .Returns(token);
    }

    protected Task<ServiceResponse<string>> LogIn(UserLogInRequestDto? request = null)
    {
        request ??= new UserLogInRequestDto()
                    {
                        LogInData = Faker.Internet.UserName(),
                        Password = Faker.Internet.Password()
                    };
        return CreateAuthService().LogIn(request);
    }
}