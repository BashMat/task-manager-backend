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

public class WhenSigningUp : AuthorizationTestBase
{
    public WhenSigningUp(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task SignUpIsSuccessful()
    {
        string userName = Faker.Internet.UserName();
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password(length: 10);
        UserSignUpRequest request = new()
                                       {
                                           UserName = userName, 
                                           Email = email,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ServiceResponse<UserSignUpResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<UserSignUpResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.UserName.Should().Be(userName);
        content.Data.Email.Should().Be(email);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfEmailIsInvalidAndStandardValidationIsPassed()
    {
        string userName = Faker.Internet.UserName();
        const string IncorrectEmail = "This is an invalid email@This is an invalid email?";
        string password = Faker.Internet.Password(length: 10);
        UserSignUpRequest request = new()
                                       {
                                           UserName = userName, 
                                           Email = IncorrectEmail,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Detail.Should().Be(AuthService.InvalidEmailAddressMessage);
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfUserNameAlreadyExists()
    {
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password(length: 10);
        UserSignUpRequest request = new()
                                       {
                                           UserName = UserName, 
                                           Email = email,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Conflict);
        content.Detail.Should().Be(AuthService.UserAlreadyExistsMessage);
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfEmailAlreadyExists()
    {
        string userName = Faker.Internet.UserName();
        string password = Faker.Internet.Password(length: 10);
        UserSignUpRequest request = new()
                                       {
                                           UserName = userName, 
                                           Email = Email,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Conflict);
        content.Detail.Should().Be(AuthService.UserAlreadyExistsMessage);
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfUserNameIsTooLong()
    {
        string userName = Faker.Random.String2(257, 1024);
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password(length: 10);
        UserSignUpRequest request = new()
                                    {
                                        UserName = userName, 
                                        Email = email,
                                        Password = password
                                    };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Title.Should().Be(ValidationErrorTitle);
    }
    
    [Fact]
    public async Task SignUpIsUnsuccessfulIfEmailIsTooLong()
    {
        string userName = Faker.Internet.UserName();
        string email = $"{Faker.Random.String2(257, 1024)}@test.test";
        string password = Faker.Internet.Password(length: 10);
        UserSignUpRequest request = new()
                                    {
                                        UserName = userName, 
                                        Email = email,
                                        Password = password
                                    };

        HttpResponseMessage response = await HttpClient.SignUp(request);
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
        string userName = Faker.Internet.UserName();
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password(length: passwordLength);
        UserSignUpRequest request = new()
                                    {
                                        UserName = userName, 
                                        Email = email,
                                        Password = password
                                    };

        HttpResponseMessage response = await HttpClient.SignUp(request);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.BadRequest);
        content.Title.Should().Be(ValidationErrorTitle);
    }
}