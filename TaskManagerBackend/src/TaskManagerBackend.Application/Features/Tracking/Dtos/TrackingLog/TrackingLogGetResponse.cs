#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;

public class TrackingLogGetResponse
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public required UserInfoDto UpdatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
    public required List<TrackingLogEntryGetResponse> TrackingLogEntries { get; init; }
    public required List<TrackingLogEntryStatusGetResponse> TrackingLogEntriesStatuses { get; init; }
}