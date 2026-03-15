using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntry;

public class UpdatableTrackingLogEntry
{
    public UpdatableTrackingLogEntry(StringAttribute title,
                                     StringAttribute? description,
                                     int trackingLogId,
                                     int statusId,
                                     int? priority,
                                     double orderIndex,
                                     int updatedBy,
                                     DateTime updatedAt)
    {
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
        StatusId = statusId;
        Priority = priority;
        OrderIndex = orderIndex;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }

    public StringAttribute Title { get; init; }
    public StringAttribute? Description { get; init; }
    public int TrackingLogId { get; init; }
    public int StatusId { get; init; }
    public int? Priority { get; init; }
    public double OrderIndex { get; init; }
    public int UpdatedBy { get; private set; }
    public DateTime UpdatedAt { get; private set; }
}