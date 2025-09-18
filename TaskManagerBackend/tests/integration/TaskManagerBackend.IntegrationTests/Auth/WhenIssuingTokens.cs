#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class WhenIssuingTokens : AuthorizationTestBase
{
    public WhenIssuingTokens(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task IssuingTokenWithPasswordGrantTypeIsSuccessful()
    {
        IssueTokenRequest request = new()
                                       {
                                           GrantType = AuthService.PasswordGrantType,
                                           UserName = UserName, 
                                           Password = Password
                                       };

        HttpResponseMessage response = await HttpClient.IssueToken(request);
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

    // TODO: Currently creates a minor time delta between requests because otherwise tokens are identical.
    //  See TODO at actual app code.
    [Fact]
    public async Task IssuingTokenWithRefreshGrantTypeIsSuccessful()
    {
        IssueTokenRequest issueTokenByPasswordRequest = new()
                                                        {
                                                            GrantType = AuthService.PasswordGrantType,
                                                            UserName = UserName,
                                                            Password = Password
                                                        };
        HttpResponseMessage responseWithInitialTokens = await HttpClient.IssueToken(issueTokenByPasswordRequest);
        ServiceResponse<IssueTokenResponse>? contentWithInitialTokens =
            await responseWithInitialTokens.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        contentWithInitialTokens.Should().NotBeNull();
        contentWithInitialTokens.Data.Should().NotBeNull();
        IssueTokenRequest issueTokenByTokenRequest = new()
                                                     {
                                                         GrantType = AuthService.RefreshTokenGrantType,
                                                         RefreshToken = contentWithInitialTokens.Data.RefreshToken
                                                     };
        HttpClient.SetAccessToken(contentWithInitialTokens.Data.AccessToken);
        Thread.Sleep(1000);

        HttpResponseMessage response = await HttpClient.IssueToken(issueTokenByTokenRequest);
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
}