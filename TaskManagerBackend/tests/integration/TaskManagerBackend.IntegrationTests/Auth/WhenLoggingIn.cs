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

public class WhenLoggingIn : AuthorizationTestBase
{
    public WhenLoggingIn(MsSqlTests fixture) : base(fixture) { }

    // TODO: Need to exclude test from autorun for CI/CD pipelines
    //[Fact]
    public async Task LoggingInIsSuccessfulWithUserName()
    {
        const string UserName = "test";
        const string Password = "test";
        UserLogInRequestDto request = new()
                                       {
                                           LogInData = UserName, 
                                           Password = Password
                                       };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ServiceResponse<string>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        //TODO: Use better approach to validate it is a JWT. 
        content.Data.Should().NotBeNull();
        content.Data.Split('.').Should().HaveCount(3);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    //[Fact]
    public async Task LoggingInIsSuccessfulWithEmail()
    {
        const string Email = "test@test.dev";
        const string Password = "test";
        UserLogInRequestDto request = new()
                                      {
                                          LogInData = Email, 
                                          Password = Password
                                      };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ServiceResponse<string>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        //TODO: Use better approach to validate it is a JWT. 
        content.Data.Should().NotBeNull();
        content.Data.Split('.').Should().HaveCount(3);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    //[Fact]
    public async Task LoggingInIsUnsuccessfulIfUserDoesNotExist()
    {
        const string UserName = "TotallyNotExistingUser";
        string password = Faker.Internet.Password();
        UserLogInRequestDto request = new()
                                       {
                                           LogInData = UserName, 
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ServiceResponse<string>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Data.Should().BeNull();
        content.Success.Should().BeFalse();
        content.Message.Should().Be(AuthService.IncorrectCredentialsMessage);
    }
    
    //[Fact]
    public async Task LoggingInIsUnsuccessfulIfPasswordIsIncorrect()
    {
        const string UserName = "test";
        const string Password = "TotallyIncorrectPassword";
        UserLogInRequestDto request = new()
                                      {
                                          LogInData = UserName, 
                                          Password = Password
                                      };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ServiceResponse<string>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<string>>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Data.Should().BeNull();
        content.Success.Should().BeFalse();
        content.Message.Should().Be(AuthService.IncorrectCredentialsMessage);
    }
}