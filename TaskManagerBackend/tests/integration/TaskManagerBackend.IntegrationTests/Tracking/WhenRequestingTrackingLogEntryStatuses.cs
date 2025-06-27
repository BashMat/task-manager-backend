#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenRequestingTrackingLogEntryStatuses : TrackingTestBase
{
    public WhenRequestingTrackingLogEntryStatuses(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task CreatingTrackingLogEntryStatusIsSuccessful()
    {
        TrackingLogGetResponse trackingLog = await CreateTrackingLogAndValidateResponse();
        const string Title = "NewLogEntryStatus";
        const string Description = "Test description";
        TrackingLogEntryStatusCreateRequest request = new()
                                                      {
                                                          TrackingLogId = trackingLog.Id,
                                                          Title = Title,
                                                          Description = Description
                                                      };

        HttpResponseMessage response = await HttpClient.CreateTrackingLogEntryStatus(request);
        ServiceResponse<TrackingLogEntryStatusGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryStatusGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.TrackingLogId.Should().Be(trackingLog.Id);
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task DeletingTrackingLogEntryStatusIsSuccessful()
    {
        TrackingLogGetResponse trackingLog = await CreateTrackingLogAndValidateResponse();
        const string Title = "NewLogEntryStatus";
        const string Description = "Test description";
        TrackingLogEntryStatusCreateRequest request = new()
                                                      {
                                                          TrackingLogId = trackingLog.Id,
                                                          Title = Title,
                                                          Description = Description
                                                      };
        HttpResponseMessage creationResponse = await HttpClient.CreateTrackingLogEntryStatus(request);
        ServiceResponse<TrackingLogEntryStatusGetResponse>? creationResponseContent = 
            await creationResponse.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryStatusGetResponse>>();
        creationResponse.EnsureSuccessStatusCode();
        creationResponseContent.Should().NotBeNull();
        creationResponseContent.Data.Should().NotBeNull();
        const string Title2 = "NewLogEntryStatus2";
        const string Description2 = "Test description2";
        TrackingLogEntryStatusCreateRequest request2 = new()
                                                      {
                                                          TrackingLogId = trackingLog.Id,
                                                          Title = Title2,
                                                          Description = Description2
                                                      };
        await HttpClient.CreateTrackingLogEntryStatus(request2);
        
        HttpResponseMessage response = 
            await HttpClient.DeleteTrackingLogEntryStatus(creationResponseContent.Data.Id);
        ServiceResponse<List<TrackingLogEntryStatusGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Where(status => status.Id == creationResponseContent.Data.Id).Should().BeEmpty();
        content.Data.Count.Should().Be(1);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}