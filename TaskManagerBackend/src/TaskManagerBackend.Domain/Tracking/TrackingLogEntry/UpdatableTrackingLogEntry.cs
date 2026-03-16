using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntry;

public class UpdatableTrackingLogEntry(StringAttribute title,
                                       StringAttribute? description,
                                       int trackingLogId,
                                       int statusId,
                                       int? priority,
                                       double orderIndex,
                                       int updatedBy,
                                       DateTime updatedAt)
{
    public StringAttribute Title { get; } = title;
    public StringAttribute? Description { get; } = description;
    public int TrackingLogId { get; } = trackingLogId;
    public int StatusId { get; } = statusId;
    public int? Priority { get; } = priority;
    public double OrderIndex { get; } = orderIndex;
    public int UpdatedBy { get; } = updatedBy;
    public DateTime UpdatedAt { get; } = updatedAt;
}