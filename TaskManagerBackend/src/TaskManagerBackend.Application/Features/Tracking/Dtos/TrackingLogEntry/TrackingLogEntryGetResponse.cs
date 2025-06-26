#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;

public class TrackingLogEntryGetResponse
{
    public int Id { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
    public int TrackingLogId { get; init; }
    public required TrackingLogEntryStatusGetResponse Status { get; init; }
    public int? Priority { get; init; }
    public double OrderIndex { get; init; }
    public required UserInfoDto CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public required UserInfoDto UpdatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
}