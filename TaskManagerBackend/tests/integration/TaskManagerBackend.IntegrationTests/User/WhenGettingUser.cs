#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.User;
using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.User;

public class WhenGettingUser : UserTestBase
{
    public WhenGettingUser(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task GettingUserDataIsSuccessfulIfDataBelongsToUser()
    {
        UserSignUpRequest signUpRequest = new() 
                                          {
                                              Email = Faker.Internet.Email(),
                                              UserName = Faker.Internet.UserName(), 
                                              Password = Faker.Internet.Password()
                                          };
        await HttpClient.SignUp(signUpRequest);
        IssueTokenRequest logInRequest = new()
                                         {
                                             GrantType = AuthService.PasswordGrantType,
                                             UserName = UserName, 
                                             Password = Password,
                                             RefreshToken = null
                                         };
        HttpResponseMessage issueTokenResponse = await HttpClient.IssueToken(logInRequest);
        ServiceResponse<IssueTokenResponse>? issueTokenContent = await issueTokenResponse.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        issueTokenContent.Should().NotBeNull();
        issueTokenContent.Data.Should().NotBeNull();
        HttpClient.SetAccessToken(issueTokenContent.Data.AccessToken);
        
        HttpResponseMessage response = await HttpClient.GetUserDataById(UserId);
        ServiceResponse<GetUserDataResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<GetUserDataResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Id.Should().Be(UserId);
        content.Data.UserName.Should().Be(UserName);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task GettingUserDataIsUnsuccessfulIfDataDoesNotBelongToUser()
    {
        UserSignUpRequest signUpRequest = new() 
                                          {
                                              Email = Faker.Internet.Email(),
                                              UserName = Faker.Internet.UserName(), 
                                              Password = Faker.Internet.Password()
                                          };
        await HttpClient.SignUp(signUpRequest);
        IssueTokenRequest logInRequest = new()
                                         {
                                             GrantType = AuthService.PasswordGrantType,
                                             UserName = signUpRequest.UserName, 
                                             Password = signUpRequest.Password,
                                             RefreshToken = null
                                         };
        HttpResponseMessage issueTokenResponse = await HttpClient.IssueToken(logInRequest);
        ServiceResponse<IssueTokenResponse>? issueTokenContent = await issueTokenResponse.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        issueTokenContent.Should().NotBeNull();
        issueTokenContent.Data.Should().NotBeNull();
        HttpClient.SetAccessToken(issueTokenContent.Data.AccessToken);
        
        HttpResponseMessage response = await HttpClient.GetUserDataById(UserId);
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();

        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(UserService.AccessDeniedMessage);
    }
}