using Microsoft.Extensions.Logging;
using Moq;
using TaskManagerBackend.Application.Services.Auth;
using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Dto.User;
using TaskManagerBackend.Tests.Common;

namespace TaskManagerBackend.Application.Tests.Auth
{
    public class AuthServiceTestBase : CommonTestBase
    {
        private const string DefaultToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZXhwIjoxNzEwNjA2MTUzfQ.Qr4baSoGgjHXUkHQ4ILRJTGBXUA4d_l7fQzV_dLV899n-K2O5hAelYl1zMM3cVEMeAk-4NwRlsJZpfb-dPMnlA";

        private readonly Mock<IAuthProvider> _authProviderMock;
        private readonly Mock<IUserRepository> _userRepositoryMock;

        protected AuthServiceTestBase()
        {
            _authProviderMock = new Mock<IAuthProvider>();
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        private AuthService CreateAuthService()
        {
            return new AuthService(_authProviderMock.Object,
                                   _userRepositoryMock.Object,
                                   Mock.Of<ILogger<AuthService>>());
        }
        
        protected void SetUpCheckIfUserExistsByUserNameOrEmail(bool userExists = false)
        {
            _userRepositoryMock.Setup(o => o.CheckIfUserExistsByUserNameOrEmail(It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync(userExists);
        }

        protected Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto request)
        {
            return CreateAuthService().SignUp(request);
        }
        
        protected void SetUpUserRepositoryMock(int? userId = null, byte[]? passwordHash = null, byte[]? passwordSalt = null)
        {
            Tuple<int, byte[], byte[]>? result = null;
            
            if (userId != null && passwordHash != null && passwordSalt != null)
            {
                result = new Tuple<int, byte[], byte[]>((int)userId, passwordHash, passwordSalt);
            }
            
            _userRepositoryMock.Setup(o => o.GetUserPasswordData(It.IsAny<string>()))
                .ReturnsAsync(result);
        }

        protected void SetUpAuthProviderMock(bool isPasswordHashCorrect = true, string token = DefaultToken)
        {
            _authProviderMock.Setup(o => o.VerifyPasswordHash(It.IsAny<string>(),
                    It.IsAny<byte[]>(),
                    It.IsAny<byte[]>()))
                .Returns(isPasswordHashCorrect);
            _authProviderMock.Setup(o => o.CreateToken(It.IsAny<int>()))
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
}