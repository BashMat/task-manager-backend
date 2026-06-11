#region Usings

using System.Net.Http.Json;
using FluentAssertions;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Utility;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests.Tracking.TrackingLogEntry;

// TODO: Add tests for problem details responses (request validation, errors during action execution)
public class WhenGettingTrackingLogEntries(MsSqlTests fixture) : TrackingTestBase(fixture)
{
    [Fact]
    public async Task GettingTrackingLogEntriesIsSuccessful()
    {
        await CreateTrackingLogEntryAndValidateResponse(DefaultTrackingLog!.Id,
                                                        DefaultTrackingLogEntryStatus!.Id);

        HttpResponseMessage response = await HttpClient.GetTrackingLogEntries();
        ServiceResponse<IReadOnlyCollection<TrackingLogEntryGetResponse>>? content = 
            await response.Content.ReadFromJsonAsync<ServiceResponse<IReadOnlyCollection<TrackingLogEntryGetResponse>>>();

        response.EnsureSuccessStatusCode();
        content.Should().NotBeNull();
        content.Data.Should().NotBeEmpty();
        content.Success.Should().BeTrue();
        content.Message.Should().BeNull();
    }
}