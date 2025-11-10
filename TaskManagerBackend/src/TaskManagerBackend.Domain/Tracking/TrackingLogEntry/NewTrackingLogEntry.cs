using TaskManagerBackend.Common.Services;

namespace TaskManagerBackend.Domain.Tracking;

public class NewTrackingLogEntry
{
    public NewTrackingLogEntry(string title,
                               string? description,
                               int trackingLogId,
                               int statusId,
                               int? priority,
                               decimal orderIndex,
                               int createdById,
                               IDateTimeService dateTimeService)
    {
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
        StatusId = statusId;
        Priority = priority;
        OrderIndex = orderIndex;
        CreatedById = createdById;
        CreatedAt = dateTimeService.UtcNow;
    }
    
    public string Title { get; }
    public string? Description { get; }
    public int TrackingLogId { get; }
    public int StatusId { get; }
    public int? Priority { get; }
    public decimal OrderIndex { get; }
    public int CreatedById { get; }
    public DateTime CreatedAt { get; }
}