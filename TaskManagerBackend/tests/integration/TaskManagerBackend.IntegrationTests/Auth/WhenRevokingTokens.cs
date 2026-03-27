#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Domain.Workflow;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class WhenRevokingTokens : AuthorizationTestBase
{
    public WhenRevokingTokens(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task IssuingTokenWithRefreshTokenGrantTypeIsUnsuccessfulIfTokensWereRevoked()
    {
        IssueTokenResponse issueTokenResponse = await HttpClient.IssueTokenByPasswordAndSetAuthorization(UserName, Password);
        await HttpClient.RevokeToken();
        
        HttpResponseMessage response = await HttpClient.IssueTokenByRefreshToken(issueTokenResponse.RefreshToken);
        ProblemDetails? content =
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Unauthorized);
        content.Detail.Should().Be(MessageResources.InvalidCredentialsMessage);
    }
}