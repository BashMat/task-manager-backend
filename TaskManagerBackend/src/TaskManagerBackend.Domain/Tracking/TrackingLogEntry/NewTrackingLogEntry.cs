using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntry;

public class NewTrackingLogEntry
{
    public NewTrackingLogEntry(StringAttribute title,
                               StringAttribute? description,
                               int trackingLogId,
                               int statusId,
                               int? priority,
                               double orderIndex,
                               int createdById,
                               DateTime createdAt)
    {
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
        StatusId = statusId;
        Priority = priority;
        OrderIndex = orderIndex;
        CreatedById = createdById;
        CreatedAt = createdAt;
    }
    
    public StringAttribute Title { get; init; }
    public StringAttribute? Description { get; init; }
    public int TrackingLogId { get; init; }
    public int StatusId { get; init; }
    public int? Priority { get; init; }
    public double OrderIndex { get; init; }
    public int CreatedById { get; init; }
    public DateTime CreatedAt { get; init; }
}