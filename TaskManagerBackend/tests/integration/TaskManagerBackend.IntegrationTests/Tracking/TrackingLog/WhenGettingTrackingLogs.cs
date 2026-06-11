#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLog;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenGettingTrackingLogs(MsSqlTests fixture) : TrackingTestBase(fixture)
{
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
}