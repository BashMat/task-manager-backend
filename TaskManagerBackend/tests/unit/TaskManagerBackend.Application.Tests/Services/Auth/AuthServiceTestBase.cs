#region Usings

using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Tests.Common;

#endregion

namespace TaskManagerBackend.Application.Tests.Services.Auth;

public class AuthServiceTestBase : UnitTestsBase
{
    protected Mock<ICryptographyService> AuthProviderMock { get; private set; }
    protected Mock<IUserRepository> UserRepositoryMock { get; private set; }
    protected Mock<IDateTimeService> DateTimeServiceMock { get; private set; }

    protected AuthServiceTestBase()
    {
        AuthProviderMock = new Mock<ICryptographyService>();
        UserRepositoryMock = new Mock<IUserRepository>();
        DateTimeServiceMock = new Mock<IDateTimeService>();
    }

    private AuthService CreateAuthService()
    {
        return new AuthService(AuthProviderMock.Object,
                               UserRepositoryMock.Object,
                               DateTimeServiceMock.Object,
                               Mock.Of<ILogger<AuthService>>());
    }
    
    protected void SetUpCreateUser(MinimalUserData createdUser)
    {
        UserRepositoryMock.Setup(o => o.CreateUser(It.IsAny<NewUser>(),
                                                   It.IsAny<CancellationToken>()))
                          .ReturnsAsync(createdUser);
    }
    
    protected void SetUpFailedUserCreation()
    {
        UserRepositoryMock.Setup(o => o.CreateUser(It.IsAny<NewUser>(),
                                                   It.IsAny<CancellationToken>()))
                          .ThrowsAsync(new Exception());
    }
        
    protected void SetUpCheckIfUserExistsByUserNameOrEmail(bool userExists)
    {
        UserRepositoryMock.Setup(o => o.CheckIfUserExistsByUsername(It.IsAny<Usernames>(),
                                                                    It.IsAny<CancellationToken>()))
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
        return CreateAuthService().SignUp(request, CancellationToken.None);
    }
}