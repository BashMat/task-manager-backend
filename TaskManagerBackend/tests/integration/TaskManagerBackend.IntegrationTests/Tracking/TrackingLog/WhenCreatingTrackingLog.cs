#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLog;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenRequestingTrackingLog(MsSqlTests fixture) : TrackingTestBase(fixture)
{
    [Fact]
    public async Task CreatingTrackingLogIsSuccessful()
    {
        DateTime utcDateTimeBeforeRequest = new DateTimeService().UtcNow;
        const string Title = "NewLog";
        const string Description = "Test description";

        HttpResponseMessage response = await CreateTrackingLog(Title, Description);
        ServiceResponse<TrackingLogGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeNull();
        content.Data.Title.Should().Be(Title);
        content.Data.Description.Should().Be(Description);
        content.Data.CreatedBy.UserName.Should().Be(UserName);
        content.Data.CreatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.UpdatedBy.UserName.Should().Be(UserName);
        content.Data.UpdatedAt.Should().BeAfter(utcDateTimeBeforeRequest);
        content.Data.TrackingLogEntries.Should().BeEmpty();
        content.Data.TrackingLogEntriesStatuses.Should().BeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }

    [Fact]
    public async Task CreatingTrackingLogIsUnsuccessfulIfTitleIsNotSet()
    {
        var request = new { Property = 1 };

        HttpResponseMessage response = await HttpClient.Post("api/tracking/logs", request);

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}