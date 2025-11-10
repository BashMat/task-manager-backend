using TaskManagerBackend.Common.Services;

namespace TaskManagerBackend.Domain.Tracking;

public class UpdatableTrackingLogEntry
{
    public UpdatableTrackingLogEntry(string title,
                                     string? description,
                                     int trackingLogId,
                                     int statusId,
                                     int? priority,
                                     double orderIndex,
                                     int updatedBy,
                                     IDateTimeService dateTimeService)
    {
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
        StatusId = statusId;
        Priority = priority;
        OrderIndex = orderIndex;
        UpdatedBy = updatedBy;
        UpdatedAt = dateTimeService.UtcNow;
    }
    
    public string Title { get; }
    public string? Description { get; }
    public int TrackingLogId { get; }
    public int StatusId { get; }
    public int? Priority { get; }
    public double OrderIndex { get; }
    public int UpdatedBy { get; }
    public DateTime UpdatedAt { get; }
}