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

    public static TrackingLogEntryStatusGetResponse ToTrackingLogEntryStatus(this TrackingLogEntryStatus trackingLogEntryTrackingLogEntryStatus)
    {
        return new TrackingLogEntryStatusGetResponse
               {
                   Id = trackingLogEntryTrackingLogEntryStatus.Id,
                   Title = trackingLogEntryTrackingLogEntryStatus.Title,
                   Description = trackingLogEntryTrackingLogEntryStatus.Description,
                   TrackingLogId = trackingLogEntryTrackingLogEntryStatus.TrackingLogId
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
                   StatusGetResponse = trackingLogEntry.TrackingLogEntryStatus.ToTrackingLogEntryStatus(),
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
                   TrackingLogEntriesStatuses = trackingLog.TrackingLogEntryStatuses.Select(s => s.ToTrackingLogEntryStatus())
                                                                                    .ToList()
               };
    }
}