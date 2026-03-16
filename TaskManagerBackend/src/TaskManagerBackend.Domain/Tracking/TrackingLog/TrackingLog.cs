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
    
    public int Id { get; }
    public StringAttribute Title { get; }
    public StringAttribute? Description { get; }
    public IReadOnlyCollection<TrackingLogEntryStatus.TrackingLogEntryStatus> TrackingLogEntryStatuses { get; }
    public IReadOnlyCollection<TrackingLogEntry.TrackingLogEntry> TrackingLogEntries { get; }
    public MinimalUserData CreatedBy { get; }
    public DateTime CreatedAt { get; }
    public MinimalUserData UpdatedBy { get; }
    public DateTime UpdatedAt { get; }
}