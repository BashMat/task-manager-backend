using FluentAssertions;
using Moq;
using TaskManagerApi.Domain;
using TaskManagerApi.Dto.User;
using Xunit;

namespace TaskManagerApi.Tests.Auth
{
    public class WhenLoggingIn : AuthServiceTestBase
    {
        private const string DefaultToken = "eyJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxIiwiZXhwIjoxNzEwNjA2MTUzfQ.Qr4baSoGgjHXUkHQ4ILRJTGBXUA4d_l7fQzV_dLV899n-K2O5hAelYl1zMM3cVEMeAk-4NwRlsJZpfb-dPMnlA";
        
        private void SetUpUserRepositoryMock(int? userId = null, byte[]? passwordHash = null, byte[]? passwordSalt = null)
        {
            Tuple<int, byte[], byte[]>? result = null;
            
            if (userId != null && passwordHash != null && passwordSalt != null)
            {
                result = new ((int)userId, passwordHash, passwordSalt);
            }
            
            userRepositoryMock.Setup(o => o.GetUserPasswordData(It.IsAny<string>()))
                              .ReturnsAsync(result);
        }

        private void SetUpAuthProviderMock(bool isPasswordHashCorrect = true,
                                             string token = DefaultToken)
        {
            authProviderMock.Setup(o => o.VerifyPasswordHash(It.IsAny<string>(),
                                                               It.IsAny<byte[]>(),
                                                               It.IsAny<byte[]>()))
                              .Returns(isPasswordHashCorrect);
            authProviderMock.Setup(o => o.CreateToken(It.IsAny<int>()))
                              .Returns(token);
        }

        private Task<ServiceResponse<string>> LogIn(UserLogInRequestDto request)
        {
            return CreateAuthService().LogIn(request);
        }
        
        [Fact]
        public async Task ServiceReturnsResponseWithMessageAndNullDataIfUserDoesNotExist()
        {
            SetUpUserRepositoryMock();
            const string TestUserName = "user";
            const string TestPassword = "email";
            UserLogInRequestDto request = new()
            {
                LogInData = TestUserName, 
                Password = TestPassword
            };

            var response = await LogIn(request);

            response.Data.Should().BeNull();
            response.Success.Should().BeFalse();
        }
        
        [Fact]
        public async Task ServiceReturnsResponseWithTokenIfCredentialsAreCorrect()
        {
            const int UserId = 1;
            byte[] passwordHash = { 1 };
            byte[] passwordSalt = { 1 };
            const string Token = "token";
            SetUpUserRepositoryMock(UserId, passwordHash, passwordSalt);
            SetUpAuthProviderMock(token: Token);
            const string TestUserName = "user";
            const string TestPassword = "email";
            UserLogInRequestDto request = new()
            {
                LogInData = TestUserName,
                Password = TestPassword
            };

            var response = await LogIn(request);

            response.Data!.Should().Be(Token);
            response.Success.Should().BeTrue();
        }
        
        [Fact]
        public async Task ServiceReturnsResponseWithMessageAndNullDataIfCredentialsAreNotCorrect()
        {
            const int UserId = 1;
            byte[] passwordHash = { 1 };
            byte[] passwordSalt = { 1 };
            SetUpUserRepositoryMock(UserId, passwordHash, passwordSalt);
            SetUpAuthProviderMock(false);
            const string TestUserName = "user";
            const string TestPassword = "email";
            UserLogInRequestDto request = new()
            {
                LogInData = TestUserName, 
                Password = TestPassword
            };

            var response = await LogIn(request);

            response.Data!.Should().BeNull();
            response.Success.Should().BeFalse();
        }
    }
}