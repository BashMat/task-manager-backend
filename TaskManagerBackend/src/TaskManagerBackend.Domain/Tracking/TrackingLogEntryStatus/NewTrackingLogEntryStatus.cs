using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

public class NewTrackingLogEntryStatus(StringAttribute title,
                                       StringAttribute? description,
                                       int trackingLogId,
                                       int createdById,
                                       DateTime createdAt)
{
    public StringAttribute Title { get; } = title;
    public StringAttribute? Description { get; } = description;
    public int TrackingLogId { get; } = trackingLogId;
    public int CreatedById { get; } = createdById;
    public DateTime CreatedAt { get; } = createdAt;
}