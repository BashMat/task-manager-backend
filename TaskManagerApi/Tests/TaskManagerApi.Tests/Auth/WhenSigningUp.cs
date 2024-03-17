using FluentAssertions;
using Moq;
using TaskManagerApi.Domain;
using TaskManagerApi.Dto.User;
using Xunit;

namespace TaskManagerApi.Tests.Auth
{
    public class WhenSigningUp : AuthServiceTestBase
    {
        private void SetUpUserRepositoryMock(bool userExists = false)
        {
            userRepositoryMock.Setup(o => o.CheckIfUserExistsByUserNameOrEmail(It.IsAny<string>(),
                                                                               It.IsAny<string>()))
                              .ReturnsAsync(userExists);
        }

        private Task<ServiceResponse<UserSignUpResponseDto>> SignUp(UserSignUpRequestDto request)
        {
            return CreateAuthService().SignUp(request);
        }
        
        [Fact]
        public async Task ServiceReturnsResponseWithMessageAndNullDataIfUserAlreadyExists()
        {
            SetUpUserRepositoryMock(true);
            const string TestUserName = "user";
            const string TestEmail = "email";
            UserSignUpRequestDto request = new()
            {
                UserName = TestUserName,
                Email = TestEmail
            };

            var response = await SignUp(request);

            response.Data.Should().BeNull();
            response.Success.Should().BeFalse();
        }

        [Fact]
        public async Task ServiceReturnsResponseWithNotNullDataIfUserDoesNotExist()
        {
            SetUpUserRepositoryMock();
            const string TestUserName = "user";
            const string TestEmail = "email";
            UserSignUpRequestDto request = new()
            {
                UserName = TestUserName, 
                Email = TestEmail
            };

            var response = await SignUp(request);

            response.Data!.UserName.Should().Be(TestUserName);
            response.Data!.Email.Should().Be(TestEmail);
            response.Success.Should().BeTrue();
        }
    }
}