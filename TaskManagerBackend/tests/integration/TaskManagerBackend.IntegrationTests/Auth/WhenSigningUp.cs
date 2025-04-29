#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application;
using TaskManagerBackend.Application.Services.Auth;
using TaskManagerBackend.Dto.User;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class WhenSigningUp : AuthorizationTestBase
{
    public WhenSigningUp(MsSqlTests fixture) : base(fixture) { }

    // TODO: Need to exclude test from autorun for CI/CD pipelines
    //[Fact]
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
    
    //[Fact]
    public async Task SignUpIsUnsuccessfulIfEmailIsInvalid()
    {
        string userName = Faker.Internet.UserName();
        const string Email = "This is an invalid email";
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
        content.Message.Should().Be(AuthService.InvalidEmailAddressMessage);
    }
    
    //[Fact]
    public async Task SignUpIsUnsuccessfulIfUserNameAlreadyExists()
    {
        const string UserName = "test";
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
    
    //[Fact]
    public async Task SignUpIsUnsuccessfulIfEmailAlreadyExists()
    {
        string userName = Faker.Internet.UserName();
        const string Email = "test@test.dev";
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