#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application;
using TaskManagerBackend.Dto.User;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Auth;

public class WhenSigningUp : AuthorizationTestBase
{
    public WhenSigningUp(MsSqlTests fixture) : base(fixture) { }

    // TODO: Need to exclude test from autorun for CI/CD pipelines
    //[Fact]
    public async Task SignUpIsSuccessful()
    {
        string userName = Faker.Internet.UserName();
        string email = Faker.Internet.Email();
        string password = Faker.Internet.Password();
        UserSignUpRequestDto request = new()
                                       {
                                           UserName = userName, 
                                           Email = email,
                                           Password = password
                                       };

        HttpResponseMessage response = await HttpClient.PostAsJsonAsync("api/auth/signup", request);

        response.EnsureSuccessStatusCode();
        ServiceResponse<UserSignUpResponseDto>? content = await response.Content.ReadFromJsonAsync<ServiceResponse<UserSignUpResponseDto>>();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.UserName.Should().Be(userName);
        content.Data.Email.Should().Be(email);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}