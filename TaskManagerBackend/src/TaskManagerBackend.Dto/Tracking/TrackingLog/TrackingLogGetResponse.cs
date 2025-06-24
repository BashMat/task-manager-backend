#region Usings

using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Dto.Tracking.TrackingLog;

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