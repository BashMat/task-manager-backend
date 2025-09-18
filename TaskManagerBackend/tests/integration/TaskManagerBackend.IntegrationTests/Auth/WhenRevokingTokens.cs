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

public class WhenRevokingTokens : AuthorizationTestBase
{
    public WhenRevokingTokens(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task IssuingTokenWithRefreshTokenGrantTypeIsUnsuccessfulIfTokensWereRevoked()
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
        await HttpClient.RevokeToken();
        
        HttpResponseMessage response = await HttpClient.IssueToken(issueTokenByTokenRequest);
        ProblemDetails? content =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Unauthorized);
        content.Detail.Should().Be(AuthService.InvalidCredentialsMessage);
    }
}