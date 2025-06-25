#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Application.Features;

public static class ConvertingExtensions
{
    public static UserInfoDto ToDto(this MinimalUserData user)
    {
        return new UserInfoDto
               {
                   Id = user.Id,
                   UserName = user.UserName,
                   Email = user.Email
               };
    }

    public static TrackingLogEntryStatusGetResponse ToDto(this Domain.Tracking.TrackingLogEntryStatus trackingLogEntryStatus)
    {
        return new TrackingLogEntryStatusGetResponse
               {
                   Id = trackingLogEntryStatus.Id,
                   Title = trackingLogEntryStatus.Title,
                   Description = trackingLogEntryStatus.Description,
                   TrackingLogId = trackingLogEntryStatus.TrackingLogId
               };
    }

    public static TrackingLogEntryGetResponse ToDto(this Domain.Tracking.TrackingLogEntry trackingLogEntry)
    {
        return new TrackingLogEntryGetResponse
               {
                   Id = trackingLogEntry.Id,
                   Title = trackingLogEntry.Title,
                   Description = trackingLogEntry.Description,
                   TrackingLogId = trackingLogEntry.TrackingLogId,
                   Status = trackingLogEntry.Status.ToDto(),
                   Priority = trackingLogEntry.Priority,
                   OrderIndex = (double)trackingLogEntry.OrderIndex,
                   CreatedAt = trackingLogEntry.CreatedAt,
                   CreatedBy = trackingLogEntry.CreatedBy.ToDto(),
                   UpdatedAt = trackingLogEntry.UpdatedAt,
                   UpdatedBy = trackingLogEntry.UpdatedBy.ToDto()
               };
    }

    public static TrackingLogGetResponse ToDto(this Domain.Tracking.TrackingLog trackingLog)
    {
        return new TrackingLogGetResponse
               {
                   Id = trackingLog.Id,
                   Title = trackingLog.Title,
                   Description = trackingLog.Description,
                   CreatedAt = trackingLog.CreatedAt,
                   CreatedBy = trackingLog.CreatedBy.ToDto(),
                   UpdatedAt = trackingLog.UpdatedAt,
                   UpdatedBy = trackingLog.UpdatedBy.ToDto(),
                   TrackingLogEntries = trackingLog.TrackingLogEntries
                                                   .Select(entry => entry.ToDto())
                                                   .ToList(),
                   TrackingLogEntriesStatuses = trackingLog.TrackingLogEntryStatuses.Select(s => s.ToDto())
                                                                                    .ToList()
               };
    }
}