#region

using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Users;
using TrackingLog = TaskManagerBackend.DataAccess.Database.Models.TrackingLog;
using TrackingLogEntry = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntry;
using TrackingLogEntryStatus = TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public static class MappingExtensions
{
    public static MinimalUserData ToDomain(this Database.Models.User user)
    {
        return new MinimalUserData(user.Id,
                                   new Usernames(StringAttribute.CreateRequired(user.UserName),
                                                 StringAttribute.CreateRequired(user.Email)));
    }

    public static TrackingLogEntryStatus.TrackingLogEntryStatus ToDomain(this Database.Models.TrackingLogEntryStatus trackingLogEntryStatus)
    {
        return new TrackingLogEntryStatus.TrackingLogEntryStatus(trackingLogEntryStatus.Id,
                                                                 StringAttribute.CreateRequired(trackingLogEntryStatus.Title),
                                                                 StringAttribute.CreateOptional(trackingLogEntryStatus.Description),
                                                                 trackingLogEntryStatus.TrackingLogId);
    }

    public static Domain.Tracking.TrackingLogEntry.TrackingLogEntry ToDomain(this TrackingLogEntry trackingLogEntry)
    {
        return new Domain.Tracking.TrackingLogEntry.TrackingLogEntry(trackingLogEntry.Id,
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

    public static Domain.Tracking.TrackingLog.TrackingLog ToDomain(this TrackingLog trackingLog)
    {
        return new Domain.Tracking.TrackingLog.TrackingLog(trackingLog.Id,
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
}