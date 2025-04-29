#region Usings

using System.Net.Http.Json;
using TaskManagerBackend.Application;
using TaskManagerBackend.Dto.User;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

public class TrackingTestBase : IntegrationTestBase,
                                IClassFixture<MsSqlTests>,
                                IAsyncLifetime,
                                IDisposable
{
    protected TrackingTestBase(MsSqlTests fixture) : base(fixture) { }
    
    public async Task InitializeAsync()
    {
        HttpResponseMessage responseMessage = await HttpClient.LogIn(new UserLogInRequestDto() 
                                                              { 
                                                                  LogInData = UserName, 
                                                                  Password = Password 
                                                              });
        ServiceResponse<string>? response = await responseMessage.Content.ReadFromJsonAsync<ServiceResponse<string>>();

        if (response is null || response.Data is null)
        {
            throw new Exception();
        }

        HttpClient.SetAccessToken(response.Data);
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }
}