﻿#region Usings

using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntry;

public class TrackingLogEntryGetResponse
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int TrackingLogId { get; set; }
    public required TrackingLogEntryStatus.TrackingLogEntryStatus Status { get; set; }
    public required UserInfoDto CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public required UserInfoDto UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}