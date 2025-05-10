#region

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.User;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

public class TrackingTestBase : IntegrationTestBase,
                                IClassFixture<MsSqlTests>,
                                IAsyncLifetime,
                                IDisposable
{
    protected TrackingTestBase(MsSqlTests fixture) : base(fixture)
    {
    }

    #region IAsyncLifetime Members

    public async Task InitializeAsync()
    {
        HttpResponseMessage responseMessage = await HttpClient.LogIn(new UserLogInRequestDto
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

    #endregion

    protected async Task<HttpResponseMessage> CreateTrackingLog(string? title = null,
                                                                string? description = null)
    {
        TrackingLogCreateRequest request = new()
                                           {
                                               Title = title ?? Faker.Lorem.Word(),
                                               Description = description ?? Faker.Lorem.Text()
                                           };

        return await HttpClient.CreateTrackingLog(request);
    }

    protected async Task<TrackingLogGetResponse> CreateTrackingLogAndValidateResponse(string? title = null,
                                                                                      string? description = null)
    {
        var creationResponse = await CreateTrackingLog();
        ServiceResponse<TrackingLogGetResponse>? creationResponseContent =
            await creationResponse.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();
        creationResponseContent.Should().NotBeNull();
        creationResponseContent.Data.Should().NotBeNull();
        return creationResponseContent.Data;
    }
}