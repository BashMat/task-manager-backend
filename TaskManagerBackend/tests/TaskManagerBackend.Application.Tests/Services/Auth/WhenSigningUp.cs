using FluentAssertions;
using TaskManagerBackend.Dto.User;
using Xunit;

namespace TaskManagerBackend.Application.Tests.Services.Auth
{
    public class WhenSigningUp : AuthServiceTestBase
    {
        [Fact]
        public async Task ServiceReturnsResponseWithMessageAndNullDataIfUserEmailAddressHasInvalidFormat()
        {
            SetUpValidateEmailAddressFormat(false);
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
        public async Task ServiceReturnsResponseWithMessageAndNullDataIfUserAlreadyExists()
        {
            SetUpValidateEmailAddressFormat();
            SetUpCheckIfUserExistsByUserNameOrEmail(true);
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
            SetUpValidateEmailAddressFormat();
            SetUpCheckIfUserExistsByUserNameOrEmail();
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