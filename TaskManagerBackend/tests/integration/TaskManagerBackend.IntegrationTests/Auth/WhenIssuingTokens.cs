#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Domain.Shared.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class WhenIssuingTokens : AuthorizationTestBase
{
    public WhenIssuingTokens(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task IssuingTokenWithPasswordGrantTypeIsSuccessfulWithUserName()
    {
        HttpResponseMessage response = await HttpClient.IssueTokenByPassword(UserName,
                                                                             Password);
        ServiceResponse<IssueTokenResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.AccessToken.Should().NotBeNullOrWhiteSpace();
        content.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
        content.Data.TokenType.Should().Be("Bearer");
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task IssuingTokenWithPasswordGrantTypeIsSuccessfulWithEmail()
    {
        HttpResponseMessage response = await HttpClient.IssueTokenByPassword(Email,
                                                                             Password);
        ServiceResponse<IssueTokenResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.AccessToken.Should().NotBeNullOrWhiteSpace();
        content.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
        content.Data.TokenType.Should().Be("Bearer");
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task IssuingTokenWithPasswordGrantTypeIsUnsuccessfulIfUserDoesNotExist()
    {
        const string IncorrectUserName = "TotallyNotExistingUser";
        string password = Faker.Internet.Password();
        
        HttpResponseMessage response = await HttpClient.IssueTokenByPassword(IncorrectUserName,
                                                                             password);
        ProblemDetails? content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Unauthorized);
        content.Detail.Should().Be(MessageResources.InvalidCredentialsMessage);
    }
    
    [Fact]
    public async Task IssuingTokenWithPasswordGrantTypeIsUnsuccessfulIfUserExistsAndPasswordIsIncorrect()
    {
        const string IncorrectPassword = "TotallyIncorrectPassword";

        HttpResponseMessage response = await HttpClient.IssueTokenByPassword(UserName,
                                                                             IncorrectPassword);
        ProblemDetails? content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Unauthorized);
        content.Detail.Should().Be(MessageResources.InvalidCredentialsMessage);
    }

    // TODO: Currently creates a minor time delta between requests because otherwise tokens are identical.
    //  See TODO at actual app code.
    [Fact]
    public async Task IssuingTokenWithRefreshGrantTypeIsSuccessful()
    {
        HttpResponseMessage responseWithInitialTokens = await HttpClient.IssueTokenByPassword(UserName,
                                                                                              Password);
        ServiceResponse<IssueTokenResponse>? contentWithInitialTokens =
            await responseWithInitialTokens.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        contentWithInitialTokens.Should().NotBeNull();
        contentWithInitialTokens.Data.Should().NotBeNull();
        contentWithInitialTokens.Data.RefreshToken.Should().NotBeNull();
        HttpClient.SetAccessToken(contentWithInitialTokens.Data.AccessToken);
        Thread.Sleep(1000);

        HttpResponseMessage response = await HttpClient.IssueTokenByRefreshToken(contentWithInitialTokens.Data.RefreshToken);
        ServiceResponse<IssueTokenResponse>? content =
            await response.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.AccessToken.Should().NotBeNullOrWhiteSpace();
        content.Data.AccessToken.Should().NotBe(contentWithInitialTokens.Data.AccessToken);
        content.Data.RefreshToken.Should().NotBeNullOrWhiteSpace();
        content.Data.RefreshToken.Should().NotBe(contentWithInitialTokens.Data.RefreshToken);
        content.Data.TokenType.Should().Be("Bearer");
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task IssuingTokenWithRefreshGrantTypeIsUnsuccessfulIfRefreshTokenIsInvalid()
    {
        string token = Faker.Random.Guid().ToString();
        HttpClient.SetAccessToken(token);
        
        HttpResponseMessage response = await HttpClient.IssueTokenByRefreshToken(token);
        ProblemDetails? content = await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Unauthorized);
        content.Detail.Should().Be(MessageResources.InvalidCredentialsMessage);
    }
}