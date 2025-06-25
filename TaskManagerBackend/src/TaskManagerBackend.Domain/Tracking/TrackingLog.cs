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
    
    public int Id { get; init; }
    public string Title { get; init; }
    public string? Description { get; init; }
    public IReadOnlyCollection<TrackingLogEntryStatus> TrackingLogEntryStatuses { get; init; }
    public IReadOnlyCollection<TrackingLogEntry> TrackingLogEntries { get; init; }
    public MinimalUserData CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public MinimalUserData UpdatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
}