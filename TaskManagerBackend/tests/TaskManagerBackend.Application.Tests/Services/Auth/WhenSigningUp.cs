#region Usings

using FluentAssertions;
using Moq;
using TaskManagerBackend.Domain.Models;
using TaskManagerBackend.Dto.User;
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
        UserSignUpRequestDto request = new()
                                       {
                                           UserName = TestUserName, 
                                           Email = TestEmail
                                       };
        User userToBeCreated = new(DateTimeServiceMock.Object,
                                   TestUserName,
                                   TestEmail,
                                   passwordHash,
                                   passwordSalt);

        await SignUp(request);

        UserRepositoryMock.Verify(o => o.Insert(It.Is<User>(createdUser 
                                                                => userToBeCreated.UserName == createdUser.UserName &&
                                                                   userToBeCreated.Email == createdUser.Email &&
                                                                   userToBeCreated.CreatedAt == createdUser.CreatedAt &&
                                                                   userToBeCreated.UpdatedAt == createdUser.UpdatedAt &&
                                                                   userToBeCreated.PasswordSalt == createdUser.PasswordSalt &&
                                                                   userToBeCreated.PasswordHash == createdUser.PasswordHash)));
    }    
}