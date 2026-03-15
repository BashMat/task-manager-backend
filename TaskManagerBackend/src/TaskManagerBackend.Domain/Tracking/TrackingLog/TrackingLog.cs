#region

using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Domain.Tracking.TrackingLog;

public class TrackingLog
{
    public TrackingLog(int id,
                       StringAttribute title,
                       StringAttribute? description,
                       MinimalUserData createdBy,
                       DateTime createdAt,
                       MinimalUserData updatedBy,
                       DateTime updatedAt,
                       IReadOnlyCollection<TrackingLogEntryStatus.TrackingLogEntryStatus> trackingLogEntryStatuses,
                       IReadOnlyCollection<TrackingLogEntry.TrackingLogEntry> trackingLogEntries)
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
    public StringAttribute Title { get; init; }
    public StringAttribute? Description { get; init; }
    public IReadOnlyCollection<TrackingLogEntryStatus.TrackingLogEntryStatus> TrackingLogEntryStatuses { get; init; }
    public IReadOnlyCollection<TrackingLogEntry.TrackingLogEntry> TrackingLogEntries { get; init; }
    public MinimalUserData CreatedBy { get; init; }
    public DateTime CreatedAt { get; init; }
    public MinimalUserData UpdatedBy { get; init; }
    public DateTime UpdatedAt { get; init; }
}