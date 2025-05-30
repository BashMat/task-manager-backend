#region

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
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
        ServiceResponse<string>? response = 
            await responseMessage.Content.ReadFromJsonAsync<ServiceResponse<string>>();

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
        HttpResponseMessage creationResponse = await CreateTrackingLog(title,
                                                                       description);
        ServiceResponse<TrackingLogGetResponse>? creationResponseContent = 
            await creationResponse.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();
        creationResponseContent.Should().NotBeNull();
        creationResponseContent.Data.Should().NotBeNull();
        return creationResponseContent.Data;
    }

    protected async Task<HttpResponseMessage> CreateTrackingLogEntryStatus(int trackingLogId,
                                                                         string? title = null,
                                                                         string? description = null)
    {
        var request = new TrackingLogEntryStatusCreateRequest()
                      {
                          TrackingLogId = trackingLogId,
                          Title = title ?? Faker.Lorem.Word(),
                          Description = description ?? Faker.Lorem.Text()
                      };

        return await HttpClient.CreateTrackingLogEntryStatus(request);
    }

    protected async Task<TrackingLogEntryStatus> CreateTrackingLogEntryStatusAndValidateResponse(int trackingLogId, 
                                                                                                 string? title = null, 
                                                                                                 string? description = null)
    {
        var creationResponse = await CreateTrackingLogEntryStatus(trackingLogId,
                                                                  title,
                                                                  description);
        var creationResponseContent =
            await creationResponse.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryStatus>>();
        creationResponseContent.Should().NotBeNull();
        creationResponseContent.Data.Should().NotBeNull();
        return creationResponseContent.Data;
    }
    
    protected async Task<HttpResponseMessage> CreateTrackingLogEntry(int trackingLogId, 
                                                                     int trackingLogEntryStatusId, 
                                                                     string? title = null, 
                                                                     string? description = null)
    {
        var request = new TrackingLogEntryCreateRequest()
                      {
                          TrackingLogId = trackingLogId,
                          StatusId = trackingLogEntryStatusId,
                          Title = title ?? Faker.Lorem.Word(),
                          Description = description ?? Faker.Lorem.Text()
                      };

        return await HttpClient.CreateTrackingLogEntry(request);
    }
    
    protected async Task<TrackingLogEntryGetResponse> CreateTrackingLogEntryAndValidateResponse(int trackingLogId, 
                                                                                                int trackingLogEntryStatusId, 
                                                                                                string? title = null, 
                                                                                                string? description = null)
    {
        var creationResponse = await CreateTrackingLogEntry(trackingLogId,
                                                            trackingLogEntryStatusId,
                                                            title,
                                                            description);
        var creationResponseContent =
            await creationResponse.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryGetResponse>>();
        creationResponseContent.Should().NotBeNull();
        creationResponseContent.Data.Should().NotBeNull();
        return creationResponseContent.Data;
    }
}