using TaskManagerBackend.Domain.Shared.Data;

namespace TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;

public class UpdatableTrackingLogEntry(StringAttribute title,
                                       StringAttribute? description,
                                       int trackingLogId,
                                       int statusId,
                                       int? priority,
                                       decimal orderIndex,
                                       int updatedBy,
                                       DateTime updatedAt)
{
    public StringAttribute Title { get; } = title;
    public StringAttribute? Description { get; } = description;
    public int TrackingLogId { get; } = trackingLogId;
    public int StatusId { get; } = statusId;
    public int? Priority { get; } = priority;
    public decimal OrderIndex { get; } = orderIndex;
    public int UpdatedBy { get; } = updatedBy;
    public DateTime UpdatedAt { get; } = updatedAt;
}