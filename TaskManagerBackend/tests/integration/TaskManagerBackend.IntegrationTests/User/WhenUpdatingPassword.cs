#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.User;
using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.User;

public class WhenUpdatingPassword : UserTestBase
{
    public WhenUpdatingPassword(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task UpdatingPasswordIsSuccessfulIfUserUpdateTheirPassword()
    {
        UserSignUpRequest signUpRequest = new() 
                                          {
                                              Email = Faker.Internet.Email(),
                                              UserName = Faker.Internet.UserName(), 
                                              Password = Faker.Internet.Password()
                                          };
        await HttpClient.SignUp(signUpRequest);
        HttpResponseMessage issueTokenResponse = await HttpClient.IssueTokenByPassword(UserName,
                                                                                       Password);
        ServiceResponse<IssueTokenResponse>? issueTokenContent = await issueTokenResponse.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        issueTokenContent.Should().NotBeNull();
        issueTokenContent.Data.Should().NotBeNull();
        HttpClient.SetAccessToken(issueTokenContent.Data.AccessToken);
        
        HttpResponseMessage response = await HttpClient.UpdatePassword(new UpdatePasswordRequest
                                                                       {
                                                                           UserId = UserId,
                                                                           OldPassword = Password,
                                                                           NewPassword = Faker.Internet.Password()
                                                                       });
        ServiceResponse<bool>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<bool>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().BeTrue();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task UpdatingPasswordIsUnsuccessfulIfUserUpdatesOtherUserPassword()
    {
        UserSignUpRequest signUpRequest = new() 
                                          {
                                              Email = Faker.Internet.Email(),
                                              UserName = Faker.Internet.UserName(), 
                                              Password = Faker.Internet.Password()
                                          };
        await HttpClient.SignUp(signUpRequest);
        HttpResponseMessage issueTokenResponse = await HttpClient.IssueTokenByPassword(UserName,
                                                                                       Password);
        ServiceResponse<IssueTokenResponse>? issueTokenContent = await issueTokenResponse.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        issueTokenContent.Should().NotBeNull();
        issueTokenContent.Data.Should().NotBeNull();
        HttpClient.SetAccessToken(issueTokenContent.Data.AccessToken);
        
        HttpResponseMessage response = await HttpClient.UpdatePassword(new UpdatePasswordRequest
                                                                       {
                                                                           UserId = 2,
                                                                           OldPassword = signUpRequest.Password,
                                                                           NewPassword = Faker.Internet.Password()
                                                                       });
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(UserService.AccessDeniedMessage);
    }
    
    [Fact]
    public async Task UpdatingPasswordIsUnsuccessfulIfUserUpdatesTheirPasswordWithIncorrectOldPassword()
    {
        UserSignUpRequest signUpRequest = new() 
                                          {
                                              Email = Faker.Internet.Email(),
                                              UserName = Faker.Internet.UserName(), 
                                              Password = Faker.Internet.Password()
                                          };
        await HttpClient.SignUp(signUpRequest);
        HttpResponseMessage issueTokenResponse = await HttpClient.IssueTokenByPassword(UserName,
                                                                                       Password);
        ServiceResponse<IssueTokenResponse>? issueTokenContent = await issueTokenResponse.Content.ReadFromJsonAsync<ServiceResponse<IssueTokenResponse>>();
        issueTokenContent.Should().NotBeNull();
        issueTokenContent.Data.Should().NotBeNull();
        HttpClient.SetAccessToken(issueTokenContent.Data.AccessToken);
        
        HttpResponseMessage response = await HttpClient.UpdatePassword(new UpdatePasswordRequest
                                                                       {
                                                                           UserId = UserId,
                                                                           OldPassword = Faker.Internet.Password(),
                                                                           NewPassword = Faker.Internet.Password()
                                                                       });
        ProblemDetails? content = 
            await response.Content.ReadFromJsonAsync<ProblemDetails>();
        
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
        content.Should().NotBeNull();
        content.Status.Should().Be((int)HttpStatusCode.Forbidden);
        content.Detail.Should().Be(UserService.AccessDeniedMessage);
    }
}