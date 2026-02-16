#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.User.Dtos;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.User;

public class WhenGettingCurrentUser : UserTestBase
{
    public WhenGettingCurrentUser(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task GettingCurrentUserDataIsSuccessful()
    {
        UserSignUpRequest signUpRequest = new() 
                                          {
                                              Email = Faker.Internet.Email(),
                                              UserName = Faker.Internet.UserName(), 
                                              Password = Faker.Internet.Password()
                                          };
        await HttpClient.SignUp(signUpRequest);
        UserLogInRequest logInRequest = new()
                                   {
                                       LogInData = UserName, 
                                       Password = Password
                                   };
        HttpResponseMessage logInResponse = await HttpClient.LogIn(logInRequest);
        ServiceResponse<string>? logInContent = await logInResponse.Content.ReadFromJsonAsync<ServiceResponse<string>>();
        logInContent.Should().NotBeNull();
        logInContent.Data.Should().NotBeNull();
        HttpClient.SetAccessToken(logInContent.Data);
        HttpResponseMessage response = await HttpClient.GetCurrentUserData();
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
}