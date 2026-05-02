using TaskManagerBackend.Domain.Shared.Data;

namespace TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntryStatus;

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