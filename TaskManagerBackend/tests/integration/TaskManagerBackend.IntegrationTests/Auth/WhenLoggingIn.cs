#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class WhenLoggingIn : AuthorizationTestBase
{
    public WhenLoggingIn(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task LoggingInIsSuccessfulWithUserName()
    {
        UserLogInRequest request = new()
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
    
    [Fact]
    public async Task LoggingInIsSuccessfulWithEmail()
    {
        UserLogInRequest request = new()
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
    
    [Fact]
    public async Task LoggingInIsUnsuccessfulIfUserDoesNotExist()
    {
        const string IncorrectUserName = "TotallyNotExistingUser";
        string password = Faker.Internet.Password();
        UserLogInRequest request = new()
                                       {
                                           LogInData = IncorrectUserName, 
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ProblemDetails? content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Unauthorized);
        content.Detail.Should().Be(AuthService.IncorrectCredentialsMessage);
    }
    
    [Fact]
    public async Task LoggingInIsUnsuccessfulIfPasswordIsIncorrect()
    {
        const string IncorrectPassword = "TotallyIncorrectPassword";
        UserLogInRequest request = new()
                                      {
                                          LogInData = UserName, 
                                          Password = IncorrectPassword
                                      };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ProblemDetails? content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Unauthorized);
        content.Detail.Should().Be(AuthService.IncorrectCredentialsMessage);
    }
    
    [Fact]
    public async Task LogInIsUnsuccessfulIfUserNameIsTooLong()
    {
        UserLogInRequest request = new()
                                   {
                                       LogInData = Faker.Random.String2(257, 1024), 
                                       Password = Password
                                   };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Title.Should().Be(ValidationErrorTitle);
    }
    
    [Fact]
    public async Task LogInIsUnsuccessfulIfEmailIsTooLong()
    {
        UserLogInRequest request = new()
                                   {
                                       LogInData = $"{Faker.Random.String2(257, 1024)}@test.test", 
                                       Password = Password
                                   };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Title.Should().Be(ValidationErrorTitle);
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    [InlineData(5)]
    [InlineData(6)]
    [InlineData(7)]
    public async Task SignUpIsUnsuccessfulIfPasswordIsTooShort(int passwordLength)
    {
        UserLogInRequest request = new()
                                   {
                                       LogInData = Email, 
                                       Password = Faker.Internet.Password(length: passwordLength)
                                   };

        HttpResponseMessage response = await HttpClient.LogIn(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Title.Should().Be(ValidationErrorTitle);
    }
}