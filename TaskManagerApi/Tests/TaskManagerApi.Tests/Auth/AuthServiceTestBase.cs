using Moq;
using TaskManagerApi.DataAccess.Repositories.User;
using TaskManagerApi.Services.Auth;

namespace TaskManagerApi.Tests.Auth
{
    public class AuthServiceTestBase
    {
        protected readonly Mock<IAuthProvider> authProviderMock;
        protected readonly Mock<IUserRepository> userRepositoryMock;

        protected AuthServiceTestBase()
        {
            authProviderMock = new Mock<IAuthProvider>();
            userRepositoryMock = new Mock<IUserRepository>();
        }

        protected AuthService CreateAuthService()
        {
            return new AuthService(authProviderMock.Object, userRepositoryMock.Object);
        }
    }
}