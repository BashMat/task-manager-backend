#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

public class WhenCreatingTrackingLog : TrackingTestBase
{
    public WhenCreatingTrackingLog(MsSqlTests fixture) : base(fixture) { }

    // TODO: Need to exclude test from autorun for CI/CD pipelines
    //[Fact]
    public async Task CreatingTrackingLogIsSuccessful()
    {
        DateTime utcDateTimeBeforeRequest = new DateTimeService().UtcNow;
        const string Title = "NewLog";
        const string Description = "Test description";
        TrackingLogCreateRequest request = new() 
                                           {
                                               Title = Title, 
                                               Description = Description
                                           };

        HttpResponseMessage response = await HttpClient.CreateTrackingLog(request);
        ServiceResponse<TrackingLogGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Data.CreatedBy.UserName.Should().Be("test");
        content.Data.CreatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.UpdatedBy.UserName.Should().Be("test");
        content.Data.UpdatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.TrackingLogEntries.Should().BeEmpty();
        content.Data.TrackingLogEntriesStatuses.Should().BeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}