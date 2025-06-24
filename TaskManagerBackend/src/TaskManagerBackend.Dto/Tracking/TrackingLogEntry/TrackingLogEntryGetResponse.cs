#region Usings

using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntry;

public class TrackingLogEntryGetResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int TrackingLogId { get; set; }
    public required TrackingLogEntryStatusGetResponse StatusGetResponse { get; set; }
    public int? Priority { get; set; }
    public double OrderIndex { get; set; }
    public required UserInfoDto CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public required UserInfoDto UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}