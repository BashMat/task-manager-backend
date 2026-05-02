#region Usings

using FluentAssertions;
using Moq;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Domain.Features.Users;
using TaskManagerBackend.Domain.Shared.Data;
using Xunit;

#endregion

namespace TaskManagerBackend.Application.Tests.Services.Auth;

public class WhenSigningUp : AuthServiceTestBase
{
    [Fact]
    public async Task ServiceReturnsResponseWithMessageAndNullDataIfUserAlreadyExists()
    {
        SetUpCheckIfUserExistsByUserNameOrEmail(true);
        SetUpFailedUserCreation();
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
        SetUpCheckIfUserExistsByUserNameOrEmail(false);
        const string TestUserName = "user";
        const string TestEmail = "email";
        const string TestPassword = "password";
        SetUpCreateUser(new MinimalUserData(1,
                                            new Usernames(StringAttribute.CreateRequired(TestUserName),
                                                          StringAttribute.CreateRequired(TestEmail))));
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
    public async Task ServiceCreatesCorrectUser()
    {
        SetUpCheckIfUserExistsByUserNameOrEmail(false);
        DateTime utcNow = Faker.Date.Between(DateTime.UtcNow.AddYears(-20), DateTime.UtcNow.AddYears(20));
        SetUpDateTimeService(utcNow);
        byte[] passwordHash = { 1 };
        byte[] passwordSalt = { 1 };
        SetUpCreatePasswordHashAndSalt(passwordHash, passwordSalt);
        const string TestUserName = "user";
        const string TestEmail = "email";
        const string TestPassword = "password";
        SetUpCreateUser(new MinimalUserData(1,
                                            new Usernames(StringAttribute.CreateRequired(TestUserName),
                                                          StringAttribute.CreateRequired(TestEmail))));
        UserSignUpRequest request = new()
                                       {
                                           UserName = TestUserName, 
                                           Email = TestEmail,
                                           Password = TestPassword
                                       };
        NewUser newUserToBeCreated = new(DateTimeServiceMock.Object,
                                         new Usernames(StringAttribute.CreateRequired(TestUserName),
                                                       StringAttribute.CreateRequired(TestEmail)),
                                         passwordHash,
                                         passwordSalt);

        ServiceResponse<UserSignUpResponse> response = await SignUp(request);

        response.Data!.UserName.Should().Be(TestUserName);
        response.Data!.Email.Should().Be(TestEmail);
        response.Success.Should().BeTrue();
        UserRepositoryMock.Verify(o => o.CreateUser(It.Is<NewUser>(createdUser
                                                                       => newUserToBeCreated.Usernames ==
                                                                          createdUser.Usernames &&
                                                                          newUserToBeCreated.CreatedAt ==
                                                                          createdUser.CreatedAt &&
                                                                          newUserToBeCreated.UpdatedAt ==
                                                                          createdUser.UpdatedAt &&
                                                                          newUserToBeCreated.PasswordSalt ==
                                                                          createdUser.PasswordSalt &&
                                                                          newUserToBeCreated.PasswordHash ==
                                                                          createdUser.PasswordHash),
                                                    It.IsAny<CancellationToken>()));
    }
}