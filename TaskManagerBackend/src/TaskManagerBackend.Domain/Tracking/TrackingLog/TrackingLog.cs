#region

using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Domain.Workflow;

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
        // TODO: Should consider splitting usage for read and write models.
        //  For example, during reading we may omit validation, expecting data from database being valid
        if (trackingLogEntryStatuses.Distinct().Count() != trackingLogEntryStatuses.Select(o => o.Id).Distinct().Count())
        {
            throw new InvariantException(ActionResultType.DataConflict,
                                         "Duplicate Tracking Log Entry Statuses are forbidden.");
        }
        
        if (trackingLogEntries.Distinct().Count() != trackingLogEntries.Select(o => o.Id).Distinct().Count())
        {
            throw new InvariantException(ActionResultType.DataConflict,
                                         "Duplicate Tracking Log Entries are forbidden.");
        }
        
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