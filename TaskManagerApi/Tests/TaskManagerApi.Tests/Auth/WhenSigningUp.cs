using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using TaskManagerApi.DataAccess.Repositories.User;
using TaskManagerApi.Domain.Dtos.User;
using TaskManagerApi.Services.Auth;
using Xunit;

namespace TaskManagerApi.Tests.Auth
{
    public class WhenSigningUp
    {
        private Mock<IUserRepository> _userRepositoryMock;
        public WhenSigningUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
        }

        private void SetUpUserRepositoryMock(bool userExists = false)
        {
            _userRepositoryMock.Setup(o => o.CheckIfUserExistsByUserNameOrEmail(It.IsAny<string>(),
                                                                                It.IsAny<string>()))
                               .ReturnsAsync(userExists);
        }
        
        [Fact]
        public async Task ServiceReturnsResponseWithMessageAndNullDataIfUserAlreadyExists()
        {
            SetUpUserRepositoryMock(true);
            AuthService sut = new(Mock.Of<IConfiguration>(), 
                                  _userRepositoryMock.Object);
            const string TestUserName = "user";
            const string TestEmail = "email";
            UserSignUpRequestDto request = new() { UserName = TestUserName, Email = TestEmail };

            var response = await sut.SignUp(request);

            response.Data.Should().Be(null);
            response.Success.Should().BeFalse();
        }
        
        [Fact]
        public async Task ServiceReturnsResponseWithNotNullDataIfUserDoesNotExist()
        {
            SetUpUserRepositoryMock();
            AuthService sut = new(Mock.Of<IConfiguration>(), 
                _userRepositoryMock.Object);
            const string TestUserName = "user";
            const string TestEmail = "email";
            UserSignUpRequestDto request = new() { UserName = TestUserName, Email = TestEmail };

            var response = await sut.SignUp(request);

            response.Data!.UserName.Should().Be(TestUserName);
            response.Data!.Email.Should().Be(TestEmail);
            response.Success.Should().BeTrue();
        }
    }
}