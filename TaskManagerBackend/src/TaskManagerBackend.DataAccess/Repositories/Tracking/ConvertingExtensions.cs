#region

using TaskManagerBackend.Domain.Users;
using TrackingLog = TaskManagerBackend.DataAccess.Database.Models.TrackingLog;
using TrackingLogEntry = TaskManagerBackend.DataAccess.Database.Models.TrackingLogEntry;
using TrackingLogEntryStatus = TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public static class ConvertingExtensions
{
    public static MinimalUserData ToDomain(this Database.Models.User user)
    {
        return new MinimalUserData(user.Id,
                                   user.UserName,
                                   user.Email);
    }

    public static TrackingLogEntryStatus ToDomain(this Database.Models.TrackingLogEntryStatus trackingLogEntryStatus)
    {
        return new TrackingLogEntryStatus(trackingLogEntryStatus.Id,
                                          trackingLogEntryStatus.Title,
                                          trackingLogEntryStatus.Description,
                                          trackingLogEntryStatus.TrackingLogId);
    }

    public static Domain.Tracking.TrackingLogEntry ToDomain(this TrackingLogEntry trackingLogEntry)
    {
        return new Domain.Tracking.TrackingLogEntry(trackingLogEntry.Id,
                                                    trackingLogEntry.Title,
                                                    trackingLogEntry.Description,
                                                    trackingLogEntry.TrackingLogId,
                                                    trackingLogEntry.TrackingLogEntryStatus.ToDomain(),
                                                    trackingLogEntry.Priority,
                                                    trackingLogEntry.OrderIndex,
                                                    trackingLogEntry.CreatedByNavigation.ToDomain(),
                                                    trackingLogEntry.CreatedAt,
                                                    trackingLogEntry.UpdatedByNavigation.ToDomain(),
                                                    trackingLogEntry.UpdatedAt);
    }

    public static Domain.Tracking.TrackingLog ToDomain(this TrackingLog trackingLog)
    {
        return new Domain.Tracking.TrackingLog(trackingLog.Id,
                                               trackingLog.Title,
                                               trackingLog.Description,
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