#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;

public class TrackingLogGetResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required UserInfoDto CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public required UserInfoDto UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public required List<TrackingLogEntryGetResponse> TrackingLogEntries { get; set; }
    public required List<TrackingLogEntryStatusGetResponse> TrackingLogEntriesStatuses { get; set; }
}