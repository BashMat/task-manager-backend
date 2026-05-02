using TaskManagerBackend.Domain.Shared.Data;

namespace TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;

public class NewTrackingLogEntry(StringAttribute title,
                                 StringAttribute? description,
                                 int trackingLogId,
                                 int statusId,
                                 int? priority,
                                 double orderIndex,
                                 int createdById,
                                 DateTime createdAt)
{
    public StringAttribute Title { get; } = title;
    public StringAttribute? Description { get; } = description;
    public int TrackingLogId { get; } = trackingLogId;
    public int StatusId { get; } = statusId;
    public int? Priority { get; } = priority;
    public double OrderIndex { get; } = orderIndex;
    public int CreatedById { get; } = createdById;
    public DateTime CreatedAt { get; } = createdAt;
}