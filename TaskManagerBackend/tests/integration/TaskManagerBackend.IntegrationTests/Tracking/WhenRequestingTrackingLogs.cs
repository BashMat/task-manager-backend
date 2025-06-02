#region Usings

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking;

public class WhenRequestingTrackingLogs : TrackingTestBase
{
    public WhenRequestingTrackingLogs(MsSqlTests fixture) : base(fixture) { }

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
    
    [Fact]
    public async Task GettingTrackingLogByIdIsSuccessful()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();

        HttpResponseMessage response = await HttpClient.GetTrackingLogById(createdTrackingLog.Id);
        ServiceResponse<TrackingLogGetResponse>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<TrackingLogGetResponse>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().BeEquivalentTo(createdTrackingLog);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task GettingTrackingLogsIsSuccessful()
    {
        await CreateTrackingLog();

        HttpResponseMessage response = await HttpClient.GetTrackingLogs();
        ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
    
    [Fact]
    public async Task DeletingTrackingLogByIdIsSuccessful()
    {
        TrackingLogGetResponse createdTrackingLog = await CreateTrackingLogAndValidateResponse();

        HttpResponseMessage response = await HttpClient.DeleteTrackingLogById(createdTrackingLog.Id);
        ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IReadOnlyCollection<TrackingLogGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotContainEquivalentOf(createdTrackingLog);
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}