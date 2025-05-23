#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

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
        ServiceResponse<TrackingLogEntryStatus>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryStatus>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.TrackingLogId.Should().Be(trackingLog.Id);
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}