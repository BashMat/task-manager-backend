#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Services.Auth;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Dto.User;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class WhenSigningUp : AuthorizationTestBase
{
    public WhenSigningUp(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task SignUpIsSuccessful()
    {
        string userName = Faker.Internet.UserName();
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password();
        UserSignUpRequestDto request = new()
                                       {
                                           UserName = userName, 
                                           Email = email,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ServiceResponse<UserSignUpResponseDto>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<UserSignUpResponseDto>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.UserName.Should().Be(userName);
        content.Data.Email.Should().Be(email);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfEmailIsInvalid()
    {
        string userName = Faker.Internet.UserName();
        const string IncorrectEmail = "This is an invalid email";
        string password = Faker.Internet.Password();
        UserSignUpRequestDto request = new()
                                       {
                                           UserName = userName, 
                                           Email = IncorrectEmail,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ServiceResponse<UserSignUpResponseDto>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<UserSignUpResponseDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Data.Should().BeNull();
        content.Success.Should().BeFalse();
        content.Message.Should().Be(AuthService.InvalidEmailAddressMessage);
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfUserNameAlreadyExists()
    {
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password();
        UserSignUpRequestDto request = new()
                                       {
                                           UserName = UserName, 
                                           Email = email,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ServiceResponse<UserSignUpResponseDto>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<UserSignUpResponseDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Data.Should().BeNull();
        content.Success.Should().BeFalse();
        content.Message.Should().Be(AuthService.UserAlreadyExistsMessage);
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfEmailAlreadyExists()
    {
        string userName = Faker.Internet.UserName();
        string password = Faker.Internet.Password();
        UserSignUpRequestDto request = new()
                                       {
                                           UserName = userName, 
                                           Email = Email,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ServiceResponse<UserSignUpResponseDto>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<UserSignUpResponseDto>>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Data.Should().BeNull();
        content.Success.Should().BeFalse();
        content.Message.Should().Be(AuthService.UserAlreadyExistsMessage);
    }
}