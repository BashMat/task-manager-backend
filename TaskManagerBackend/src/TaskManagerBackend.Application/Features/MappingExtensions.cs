#region Usings

using TaskManagerBackend.Application.Features.Auth.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Domain.Features.Users;

#endregion

namespace TaskManagerBackend.Application.Features;

public static class MappingExtensions
{
    public static UserInfoDto ToDto(this MinimalUserData user)
    {
        return new UserInfoDto
               {
                   Id = user.Id,
                   UserName = user.Usernames.AccountName.Value,
                   Email = user.Usernames.Email.Value
               };
    }

    public static TrackingLogEntryStatusGetResponse ToDto(this TrackingLogEntryStatus trackingLogEntryStatus)
    {
        return new TrackingLogEntryStatusGetResponse
               {
                   Id = trackingLogEntryStatus.Id,
                   Title = trackingLogEntryStatus.Title.Value,
                   Description = trackingLogEntryStatus.Description?.Value,
                   TrackingLogId = trackingLogEntryStatus.TrackingLogId
               };
    }

    public static TrackingLogEntryGetResponse ToDto(this TrackingLogEntry trackingLogEntry)
    {
        return new TrackingLogEntryGetResponse
               {
                   Id = trackingLogEntry.Id,
                   Title = trackingLogEntry.Title.Value,
                   Description = trackingLogEntry.Description?.Value,
                   TrackingLogId = trackingLogEntry.TrackingLogId,
                   Status = trackingLogEntry.Status.ToDto(),
                   Priority = trackingLogEntry.Priority,
                   OrderIndex = trackingLogEntry.OrderIndex,
                   CreatedAt = trackingLogEntry.CreatedAt,
                   CreatedBy = trackingLogEntry.CreatedBy.ToDto(),
                   UpdatedAt = trackingLogEntry.UpdatedAt,
                   UpdatedBy = trackingLogEntry.UpdatedBy.ToDto()
               };
    }

    public static TrackingLogGetResponse ToDto(this TrackingLog trackingLog)
    {
        return new TrackingLogGetResponse
               {
                   Id = trackingLog.Id,
                   Title = trackingLog.Title.Value,
                   Description = trackingLog.Description?.Value,
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