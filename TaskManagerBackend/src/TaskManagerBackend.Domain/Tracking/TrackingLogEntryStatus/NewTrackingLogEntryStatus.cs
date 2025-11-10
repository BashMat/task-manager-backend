using TaskManagerBackend.Common.Services;

namespace TaskManagerBackend.Domain.Tracking;

public class NewTrackingLogEntryStatus
{
    public NewTrackingLogEntryStatus(string title,
                                     string? description,
                                     int trackingLogId,
                                     int createdById,
                                     IDateTimeService dateTimeService)
    {
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
        CreatedById = createdById;
        CreatedAt = dateTimeService.UtcNow;
    }

    public string Title { get; }
    public string? Description { get; }
    public int TrackingLogId { get; }
    public int CreatedById { get; }
    public DateTime CreatedAt { get; }
}