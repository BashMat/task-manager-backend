#region

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

public class TrackingTestBase : IntegrationTestBase,
                                IClassFixture<MsSqlTests>,
                                IAsyncLifetime,
                                IDisposable
{
    protected TrackingLogGetResponse? DefaultTrackingLog;
    protected TrackingLogEntryStatusGetResponse? DefaultTrackingLogEntryStatus;
    
    protected TrackingTestBase(MsSqlTests fixture) : base(fixture)
    {
    }

    #region IAsyncLifetime Members

    // TODO: Improve initialization. Create additional users, store multiple users and their Tracking Logs, test everything
    public virtual async Task InitializeAsync()
    {
        HttpResponseMessage responseMessage = await HttpClient.LogIn(new UserLogInRequest
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

        int existingLogsCount = Faker.Random.Int(min: 5, max: 10);
        int defaultLogCount = Faker.Random.Int(min: 0, max: existingLogsCount - 1);

        for (int i = 0; i < existingLogsCount; i++)
        {
            TrackingLogGetResponse log = await CreateTrackingLogAndValidateResponse();
            if (i == defaultLogCount)
            {
                DefaultTrackingLog = log;
            }
            
            int existingStatusesCount = Faker.Random.Int(min: 2, max: 10);
            int defaultStatusCount = Faker.Random.Int(min: 0, max: existingStatusesCount - 1);
            
            for (int j = 0; j < existingStatusesCount; j++)
            {
                TrackingLogEntryStatusGetResponse status = 
                    await CreateTrackingLogEntryStatusAndValidateResponse(log.Id);
                if (i == defaultLogCount && j == defaultStatusCount)
                {
                    DefaultTrackingLogEntryStatus = status;
                }
            
                int existingEntriesCount = Faker.Random.Int(min: 2, max: 10);
                for (int k = 0; k < existingEntriesCount; k++)
                {
                    await CreateTrackingLogEntryAndValidateResponse(log.Id,
                                                                    status.Id);
                }
            }
        }
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

    protected async Task<TrackingLogEntryStatusGetResponse> CreateTrackingLogEntryStatusAndValidateResponse(int trackingLogId, 
                                                                                                 string? title = null, 
                                                                                                 string? description = null)
    {
        var creationResponse = await CreateTrackingLogEntryStatus(trackingLogId,
                                                                  title,
                                                                  description);
        var creationResponseContent =
            await creationResponse.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryStatusGetResponse>>();
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