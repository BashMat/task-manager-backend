using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

public class NewTrackingLogEntryStatus
{
    public NewTrackingLogEntryStatus(StringAttribute title,
                                     StringAttribute? description,
                                     int trackingLogId,
                                     int createdById,
                                     DateTime createdAt)
    {
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
        CreatedById = createdById;
        CreatedAt = createdAt;
    }
    
    public StringAttribute Title { get; init; }
    public StringAttribute? Description { get; init; }
    public int TrackingLogId { get; init; }
    public int CreatedById { get; init; }
    public DateTime CreatedAt { get; init; }
}