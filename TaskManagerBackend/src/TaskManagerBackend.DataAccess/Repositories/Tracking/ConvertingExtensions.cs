#region

using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Dto.User;

#endregion

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public static class ConvertingExtensions
{
    public static UserInfoDto ToUserInfoDto(this Database.Models.User user)
    {
        return new UserInfoDto
               {
                   Id = user.Id,
                   UserName = user.UserName,
                   FirstName = user.FirstName,
                   LastName = user.LastName,
                   Email = user.Email
               };
    }

    public static TrackingLogEntryStatus ToTrackingLogEntryStatus(this Status trackingLogEntryStatus)
    {
        return new TrackingLogEntryStatus
               {
                   Id = trackingLogEntryStatus.Id,
                   Title = trackingLogEntryStatus.Title,
                   Description = trackingLogEntryStatus.Description,
                   TrackingLogId = trackingLogEntryStatus.TrackingLogId
               };
    }

    public static TrackingLogEntryGetResponse ToTrackingLogEntryGetResponse(this TrackingLogEntry trackingLogEntry)
    {
        return new TrackingLogEntryGetResponse
               {
                   Id = trackingLogEntry.Id,
                   Title = trackingLogEntry.Title,
                   Description = trackingLogEntry.Description,
                   TrackingLogId = trackingLogEntry.TrackingLogId,
                   Status = trackingLogEntry.Status.ToTrackingLogEntryStatus(),
                   Priority = trackingLogEntry.Priority,
                   OrderIndex = (double)trackingLogEntry.OrderIndex,
                   CreatedAt = trackingLogEntry.CreatedAt,
                   CreatedBy = trackingLogEntry.CreatedByNavigation.ToUserInfoDto(),
                   UpdatedAt = trackingLogEntry.UpdatedAt,
                   UpdatedBy = trackingLogEntry.UpdatedByNavigation.ToUserInfoDto()
               };
    }

    public static TrackingLogGetResponse ToTrackingLogGetResponse(this TrackingLog trackingLog)
    {
        return new TrackingLogGetResponse
               {
                   Id = trackingLog.Id,
                   Title = trackingLog.Title,
                   Description = trackingLog.Description,
                   CreatedAt = trackingLog.CreatedAt,
                   CreatedBy = trackingLog.CreatedByNavigation.ToUserInfoDto(),
                   UpdatedAt = trackingLog.UpdatedAt,
                   UpdatedBy = trackingLog.UpdatedByNavigation.ToUserInfoDto(),
                   TrackingLogEntries = trackingLog.TrackingLogEntries
                                                   .Select(entry => entry.ToTrackingLogEntryGetResponse())
                                                   .ToList(),
                   TrackingLogEntriesStatuses = trackingLog.Statuses.Select(s => s.ToTrackingLogEntryStatus())
                                                           .ToList()
               };
    }
}