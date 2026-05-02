#region

using TaskManagerBackend.Domain.Features.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Features.Users;
using TaskManagerBackend.Domain.Shared.Data;
using TrackingLog = TaskManagerBackend.DataAccess.Database.Models.TrackingLog;
using TrackingLogEntry = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntry;
using TrackingLogEntryStatus_TrackingLogEntryStatus = TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntryStatus.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.DataAccess.Features.Tracking;

public static class MappingExtensions
{
    public static MinimalUserData ToDomain(this Database.Models.User user)
    {
        return new MinimalUserData(user.Id,
                                   new Usernames(StringAttribute.CreateRequired(user.UserName),
                                                 StringAttribute.CreateRequired(user.Email)));
    }

    public static TrackingLogEntryStatus_TrackingLogEntryStatus ToDomain(this Database.Models.TrackingLogEntryStatus trackingLogEntryStatus)
    {
        return new TrackingLogEntryStatus_TrackingLogEntryStatus(trackingLogEntryStatus.Id,
                                                                 StringAttribute.CreateRequired(trackingLogEntryStatus.Title),
                                                                 StringAttribute.CreateOptional(trackingLogEntryStatus.Description),
                                                                 trackingLogEntryStatus.TrackingLogId);
    }

    public static Domain.Features.Tracking.TrackingLogEntry.TrackingLogEntry ToDomain(this TrackingLogEntry trackingLogEntry)
    {
        return new Domain.Features.Tracking.TrackingLogEntry.TrackingLogEntry(trackingLogEntry.Id,
                                                                              StringAttribute.CreateRequired(trackingLogEntry.Title),
                                                                              StringAttribute.CreateOptional(trackingLogEntry.Description),
                                                                              trackingLogEntry.TrackingLogId,
                                                                              trackingLogEntry.TrackingLogEntryStatus.ToDomain(),
                                                                              trackingLogEntry.Priority,
                                                                              trackingLogEntry.OrderIndex,
                                                                              trackingLogEntry.CreatedByNavigation.ToDomain(),
                                                                              trackingLogEntry.CreatedAt,
                                                                              trackingLogEntry.UpdatedByNavigation.ToDomain(),
                                                                              trackingLogEntry.UpdatedAt);
    }

    public static Domain.Features.Tracking.TrackingLog.TrackingLog ToDomain(this TrackingLog trackingLog)
    {
        return new Domain.Features.Tracking.TrackingLog.TrackingLog(trackingLog.Id,
                                                                    StringAttribute.CreateRequired(trackingLog.Title),
                                                                    StringAttribute.CreateOptional(trackingLog.Description),
                                                                    trackingLog.CreatedByNavigation.ToDomain(),
                                                                    trackingLog.CreatedAt,
                                                                    trackingLog.UpdatedByNavigation.ToDomain(),
                                                                    trackingLog.UpdatedAt,
                                                                    trackingLog.TrackingLogEntryStatuses.Select(s => s.ToDomain())
                                                                               .ToList(),
                                                                    trackingLog.TrackingLogEntries
                                                                               .Select(entry => entry.ToDomain())
                                                                               .ToList());
    }
    
    public static TrackingLogEntity ToDomainEntity(this TrackingLog trackingLog)
    {
        return new TrackingLogEntity(trackingLog.Id,
                                     StringAttribute.CreateRequired(trackingLog.Title),
                                     StringAttribute.CreateOptional(trackingLog.Description),
                                     trackingLog.CreatedBy,
                                     trackingLog.CreatedAt,
                                     trackingLog.UpdatedBy,
                                     trackingLog.UpdatedAt);
    }
}