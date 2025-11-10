#region

using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLog
{
    public TrackingLog(int id,
                       string title,
                       string? description,
                       MinimalUserData createdBy,
                       DateTime createdAt,
                       MinimalUserData updatedBy,
                       DateTime updatedAt,
                       IReadOnlyCollection<TrackingLogEntryStatus> trackingLogEntryStatuses,
                       IReadOnlyCollection<TrackingLogEntry> trackingLogEntries)
    {
        Id = id;
        Title = title;
        Description = description;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
        TrackingLogEntryStatuses = trackingLogEntryStatuses;
        TrackingLogEntries = trackingLogEntries;
    }
    
    public int Id { get; }
    public string Title { get; }
    public string? Description { get; }
    public IReadOnlyCollection<TrackingLogEntryStatus> TrackingLogEntryStatuses { get; }
    public IReadOnlyCollection<TrackingLogEntry> TrackingLogEntries { get; }
    public MinimalUserData CreatedBy { get; }
    public DateTime CreatedAt { get; }
    public MinimalUserData UpdatedBy { get; }
    public DateTime UpdatedAt { get; }
}