#region Usings

using FluentAssertions;
using Moq;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Domain.Users;
using Xunit;

#endregion

namespace TaskManagerBackend.Application.Tests.Services.Auth;

public class WhenSigningUp : AuthServiceTestBase
{
    [Fact]
    public async Task ServiceReturnsResponseWithMessageAndNullDataIfUserEmailAddressHasInvalidFormat()
    {
        SetUpValidateEmailAddressFormat(false);
        const string TestUserName = "user";
        const string TestEmail = "email";
        const string TestPassword = "password";
        UserSignUpRequest request = new()
                                       {
                                           UserName = TestUserName, 
                                           Email = TestEmail,
                                           Password = TestPassword
                                       };

        ServiceResponse<UserSignUpResponse> response = await SignUp(request);

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
        const string TestPassword = "password";
        UserSignUpRequest request = new()
                                       {
                                           UserName = TestUserName, 
                                           Email = TestEmail,
                                           Password = TestPassword
                                       };

        ServiceResponse<UserSignUpResponse> response = await SignUp(request);

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
        const string TestPassword = "password";
        UserSignUpRequest request = new()
                                       {
                                           UserName = TestUserName, 
                                           Email = TestEmail,
                                           Password = TestPassword
                                       };

        ServiceResponse<UserSignUpResponse> response = await SignUp(request);

        response.Data!.UserName.Should().Be(TestUserName);
        response.Data!.Email.Should().Be(TestEmail);
        response.Success.Should().BeTrue();
    }
        
    [Fact]
    public async Task ServiceInsertsCorrectUser()
    {
        SetUpValidateEmailAddressFormat();
        SetUpCheckIfUserExistsByUserNameOrEmail();
        DateTime utcNow = Faker.Date.Between(DateTime.UtcNow.AddYears(-20), DateTime.UtcNow.AddYears(20));
        SetUpDateTimeService(utcNow);
        byte[] passwordHash = { 1 };
        byte[] passwordSalt = { 1 };
        SetUpCreatePasswordHashAndSalt(passwordHash, passwordSalt);
        const string TestUserName = "user";
        const string TestEmail = "email";
        const string TestPassword = "password";
        UserSignUpRequest request = new()
                                       {
                                           UserName = TestUserName, 
                                           Email = TestEmail,
                                           Password = TestPassword
                                       };
        NewUser newUserToBeCreated = new(DateTimeServiceMock.Object,
                                   TestUserName,
                                   TestEmail,
                                   passwordHash,
                                   passwordSalt);

        await SignUp(request);

        UserRepositoryMock.Verify(o => o.InsertUser(It.Is<NewUser>(createdUser 
                                                                => newUserToBeCreated.UserName == createdUser.UserName &&
                                                                   newUserToBeCreated.Email == createdUser.Email &&
                                                                   newUserToBeCreated.CreatedAt == createdUser.CreatedAt &&
                                                                   newUserToBeCreated.UpdatedAt == createdUser.UpdatedAt &&
                                                                   newUserToBeCreated.PasswordSalt == createdUser.PasswordSalt &&
                                                                   newUserToBeCreated.PasswordHash == createdUser.PasswordHash)));
    }
}