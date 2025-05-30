#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

public class WhenRequestingTrackingLogEntries : TrackingTestBase
{
    public WhenRequestingTrackingLogEntries(MsSqlTests fixture) : base(fixture) { }

    [Fact]
    public async Task CreatingTrackingLogEntryIsSuccessful()
    {
        TrackingLogGetResponse trackingLog = await CreateTrackingLogAndValidateResponse();
        TrackingLogEntryStatus trackingLogEntryStatus = 
            await CreateTrackingLogEntryStatusAndValidateResponse(trackingLog.Id);
        const string Title = "NewLogEntry";
        const string Description = "Test description";
        DateTime utcDateTimeBeforeRequest = new DateTimeService().UtcNow;

        HttpResponseMessage response = await CreateTrackingLogEntry(trackingLog.Id,
                                                                    trackingLogEntryStatus.Id,
                                                                    Title,
                                                                    Description);
        ServiceResponse<TrackingLogEntryGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogEntryGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Data.CreatedBy.UserName.Should().Be("test");
        content.Data.CreatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.UpdatedBy.UserName.Should().Be("test");
        content.Data.UpdatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.TrackingLogId.Should().Be(trackingLog.Id);
        content.Data.Status.Should().BeEquivalentTo(trackingLogEntryStatus);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }

    [Fact]
    public async Task CreatingTrackingLogEntryIsUnsuccessfulIfTitleIsNotSet()
    {
        var request = new { Property = 1 };

        HttpResponseMessage response = await HttpClient.Post("api/tracking/log-entries", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}