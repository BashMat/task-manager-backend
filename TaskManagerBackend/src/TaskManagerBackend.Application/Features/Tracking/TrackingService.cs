#region Usings

using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Features.Tracking;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Domain.Shared.Data;
using TaskManagerBackend.Domain.Shared.Entities;
using TaskManagerBackend.Domain.Shared.Workflow;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking;

public class TrackingService(ITrackingRepository trackingRepository,
                             IDateTimeService dateTimeService) : ITrackingService
{
    #region Tracking Logs

    public async Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(TrackingLogCreateRequest request,
                                                                                 int userId,
                                                                                 CancellationToken cancellationToken)
    {
        NewTrackingLog newLog = new(StringAttribute.CreateRequired(request.Title),
                                    StringAttribute.CreateOptional(request.Description),
                                    userId,
                                    dateTimeService.UtcNow);

        TrackingLog? log = await trackingRepository.CreateTrackingLog(newLog, cancellationToken);

        if (log is null)
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResultType: ActionResultType.ServerError,
                                                               message: MessageResources.CouldNotCreateMessage);
        }

        return log.ToDto();
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogsByUserId(int userId,
                                                                                                CancellationToken cancellationToken)
    {
        List<TrackingLog> logs = await trackingRepository.GetAllTrackingLogs(userId, cancellationToken);
        return logs.Select(l => l.ToDto()).ToList();
    }

    public async Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id,
                                                                                  int userId,
                                                                                  CancellationToken cancellationToken)
    {
        TrackingLog? log = await trackingRepository.GetTrackingLogById(id, cancellationToken);
        
        if (log is null)
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                               message: MessageResources.ResourceDoesNotExist);
        }
        
        if (!CanGetTrackingEntity(log, userId))
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResultType: ActionResultType.Unauthorized,
                                                               message: MessageResources.AccessDeniedMessage);
        }
        
        return log.ToDto();
    }

    public async Task<ServiceResponse<TrackingLogGetResponse>> EditTrackingLog(TrackingLogEditRequest request,
                                                                               int userId,
                                                                               CancellationToken cancellationToken)
    {
        TrackingLogEntity? log = await trackingRepository.GetTrackingLogEntityById(request.Id, cancellationToken);
        
        if (log is null)
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                               message: MessageResources.ResourceDoesNotExist);
        }
        
        if (!CanEditTrackingEntity(log, userId))
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResultType: ActionResultType.Unauthorized,
                                                               message: MessageResources.AccessDeniedMessage);
        }

        switch (request.Title)
        {
            case { HasValue: true, Value: null }:
                return new ServiceResponse<TrackingLogGetResponse>(actionResultType: ActionResultType.UserError,
                                                                   message: MessageResources.ValidationErrorTitle);
            case { HasValue: true, Value: not null }:
            {
                StringAttribute targetTitle = StringAttribute.CreateRequired(request.Title.Value);
                log.RenameToByUser(targetTitle,
                                   userId,
                                   dateTimeService.UtcNow);
                break;
            }
        }

        if (request.Description.HasValue)
        {
            StringAttribute? targetDescription = StringAttribute.CreateOptional(request.Description.Value);
        
            log.EditDescriptionToByUser(targetDescription, 
                                        userId, 
                                        dateTimeService.UtcNow);
        }
        
        await trackingRepository.Save(log, cancellationToken);

        return await GetTrackingLogById(log.Id, userId, cancellationToken);
    }
    
    private static bool CanGetTrackingEntity(IAuditedEntityWithMinimalUserData trackingEntity, int userId)
    {
        return trackingEntity.CreatedBy.Id == userId;
    }
    
    private static bool CanEditTrackingEntity(IAuditedEntityWithMinimalUserData trackingEntity, int userId)
    {
        return CanGetTrackingEntity(trackingEntity, userId);
    }

    private static bool CanEditTrackingEntity(IAuditedEntity trackingEntity, int userId)
    {
        return trackingEntity.CreatedBy == userId;
    }
    
    private async Task<ServiceResponse<T>?> CanCreateTrackingLogChildEntity<T>(int trackingLogId,
                                                                               int userId,
                                                                               CancellationToken cancellationToken)
    {
        TrackingLogEntity? trackingLog = await trackingRepository.GetTrackingLogEntityById(trackingLogId,
                                                                                           cancellationToken);

        if (trackingLog is null)
        {
            return new ServiceResponse<T>(actionResultType: ActionResultType.UserError,
                                          message: MessageResources.CouldNotCreateMessage);
        }

        if (!CanEditTrackingEntity(trackingLog, userId))
        {
            return new ServiceResponse<T>(actionResultType: ActionResultType.Unauthorized,
                                          message: MessageResources.AccessDeniedMessage);
        }

        return null;
    }
    
    private async Task<ServiceResponse<T>?> CanActOnTrackingLogChildEntity<T>(int trackingLogId,
                                                                              int userId,
                                                                              CancellationToken cancellationToken)
    {
        TrackingLogEntity? trackingLog = await trackingRepository.GetTrackingLogEntityById(trackingLogId,
                                                                                           cancellationToken);

        if (trackingLog is null)
        {
            return new ServiceResponse<T>(actionResultType: ActionResultType.UserError,
                                          message: MessageResources.ValidationErrorTitle);
        }

        if (!CanEditTrackingEntity(trackingLog, userId))
        {
            return new ServiceResponse<T>(actionResultType: ActionResultType.Unauthorized,
                                          message: MessageResources.AccessDeniedMessage);
        }

        return null;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int id, int userId,
                                                                                           CancellationToken cancellationToken)
    {
        List<TrackingLog> logs = await trackingRepository.DeleteTrackingLogById(id, userId, cancellationToken);
        return logs.Select(l => l.ToDto()).ToList();
    }

    #endregion

    #region Tracking Log Entries

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> CreateTrackingLogEntry(TrackingLogEntryCreateRequest request,
                                                                                           int userId,
                                                                                           CancellationToken cancellationToken)
    {
        ServiceResponse<TrackingLogEntryGetResponse>? failedResultOrNull = 
            await CanCreateTrackingLogChildEntity<TrackingLogEntryGetResponse>(request.TrackingLogId, userId, cancellationToken);

        if (failedResultOrNull is not null)
        {
            return failedResultOrNull;
        }

        TrackingLogEntryStatus? status = await trackingRepository.GetTrackingLogEntryStatusById(request.StatusId,
                                                                                                cancellationToken);

        if (status is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.UserError,
                                                                    message: MessageResources.CouldNotCreateMessage);
        }
        
        NewTrackingLogEntry newTrackingLogEntry = new(StringAttribute.CreateRequired(request.Title),
                                                      StringAttribute.CreateOptional(request.Description),
                                                      request.TrackingLogId,
                                                      request.StatusId,
                                                      request.Priority,
                                                      request.OrderIndex,
                                                      userId,
                                                      dateTimeService.UtcNow);
        
        TrackingLogEntry? entry = await trackingRepository.CreateTrackingLogEntry(newTrackingLogEntry, cancellationToken);

        if (entry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.ServerError,
                                                                    message: MessageResources.CouldNotCreateMessage);
        }

        return entry.ToDto();
    }

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> GetAllTrackingLogEntriesByUserId(int userId, 
                                                                                                           CancellationToken cancellationToken)
    {
        List<TrackingLogEntry> entries = await trackingRepository.GetAllTrackingLogEntries(userId, cancellationToken);
        return entries.Select(e => e.ToDto()).ToList();
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> GetTrackingLogEntryById(int id,
                                                                                            int userId,
                                                                                            CancellationToken cancellationToken)
    {
        TrackingLogEntry? entry = await trackingRepository.GetTrackingLogEntryById(id, cancellationToken);
        
        if (entry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                                    message: MessageResources.ResourceDoesNotExist);
        }
        
        if (!CanGetTrackingEntity(entry, userId))
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.Unauthorized,
                                                                    message: MessageResources.AccessDeniedMessage);
        }

        return entry.ToDto();
    }

    [Obsolete("Use specialized actions instead of single update action")]
    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntryById(int id,
                                                                                               UpdateTrackingLogEntryRequest request,
                                                                                               int userId,
                                                                                               CancellationToken cancellationToken)
    {
        TrackingLogEntry? entry = await trackingRepository.GetTrackingLogEntryById(id, cancellationToken);

        if (entry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                                    message: MessageResources.ResourceDoesNotExist);
        }
        
        if (!CanEditTrackingEntity(entry, userId))
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.Unauthorized,
                                                                    message: MessageResources.AccessDeniedMessage);
        }

        // TODO: when resource is created and updated locally immediately,
        // DateTime value is considered equal.
        // Perhaps some sort of more accurate timestamp should be used
        if (entry.UpdatedAt > request.UpdatedAt)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.DataConflict,
                                                                    message: MessageResources.UpdateConflict);
        }

        if (entry.TrackingLogId != request.TrackingLogId)
        {
            ServiceResponse<TrackingLogEntryGetResponse>? failedResultOrNull = 
                await CanActOnTrackingLogChildEntity<TrackingLogEntryGetResponse>(request.TrackingLogId, userId, cancellationToken);

            if (failedResultOrNull is not null)
            {
                return failedResultOrNull;
            }
        }

        UpdatableTrackingLogEntry updatableTrackingLogEntry = new(StringAttribute.CreateRequired(request.Title),
                                                                  StringAttribute.CreateOptional(request.Description),
                                                                  request.TrackingLogId,
                                                                  request.StatusId,
                                                                  request.Priority,
                                                                  request.OrderIndex,
                                                                  userId,
                                                                  dateTimeService.UtcNow);

        TrackingLogEntry? updatedEntry = 
            await trackingRepository.UpdateTrackingLogEntryById(id, updatableTrackingLogEntry, cancellationToken);
        
        if (updatedEntry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                                    message: MessageResources.ResourceDoesNotExist);
        }
        
        return updatedEntry.ToDto();
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> MoveTrackingLogEntry(TrackingLogEntryMoveRequest request,
                                                                                         int userId,
                                                                                         CancellationToken cancellationToken)
    {
        if (request.TrackingLogEntryStatusId is { HasValue: false } &&
            request.OrderIndex is { HasValue: false })
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.UserError,
                                                                    message: MessageResources.ValidationErrorTitle);
        }
        
        TrackingLogEntry? entry = await trackingRepository.GetTrackingLogEntryById(request.Id, cancellationToken);

        if (entry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                                    message: MessageResources.ResourceDoesNotExist);
        }
        
        if (!CanEditTrackingEntity(entry, userId))
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.Unauthorized,
                                                                    message: MessageResources.AccessDeniedMessage);
        }

        TrackingLogEntryStatus? targetTrackingLogEntryStatusToMoveTo = null;
        if (request.TrackingLogEntryStatusId is { HasValue: true } &&
            entry.Status.Id != request.TrackingLogEntryStatusId.Value)
        {
            targetTrackingLogEntryStatusToMoveTo =
                await trackingRepository.GetTrackingLogEntryStatusById(request.TrackingLogEntryStatusId.HasValue
                                                                           ? request.TrackingLogEntryStatusId
                                                                                    .Value
                                                                           : entry.Status.Id,
                                                                       cancellationToken);

            if (targetTrackingLogEntryStatusToMoveTo is null)
            {
                return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.UserError,
                                                                        message: MessageResources.ValidationErrorTitle);
            }

            if (entry.TrackingLogId != targetTrackingLogEntryStatusToMoveTo.TrackingLogId)
            {
                ServiceResponse<TrackingLogEntryGetResponse>? failedResultOrNull = 
                    await CanActOnTrackingLogChildEntity<TrackingLogEntryGetResponse>(targetTrackingLogEntryStatusToMoveTo.TrackingLogId,
                                                                                      userId,
                                                                                      cancellationToken);

                if (failedResultOrNull is not null)
                {
                    return failedResultOrNull;
                }
            }
        }

        int targetTrackingLogId = targetTrackingLogEntryStatusToMoveTo?.TrackingLogId ?? entry.TrackingLogId;
        int targetTrackingLogEntryStatusId = targetTrackingLogEntryStatusToMoveTo?.Id ?? entry.Status.Id;

        UpdatableTrackingLogEntry updatableTrackingLogEntry = new(entry.Title,
                                                                  entry.Description,
                                                                  targetTrackingLogId,
                                                                  targetTrackingLogEntryStatusId,
                                                                  entry.Priority,
                                                                  request.OrderIndex is { HasValue: true }
                                                                      ? request.OrderIndex.Value
                                                                      : entry.OrderIndex,
                                                                  userId,
                                                                  dateTimeService.UtcNow);

        TrackingLogEntry? updatedEntry = 
            await trackingRepository.UpdateTrackingLogEntryById(request.Id, updatableTrackingLogEntry, cancellationToken);
        
        if (updatedEntry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                                    message: MessageResources.ResourceDoesNotExist);
        }
        
        return updatedEntry.ToDto();
    }

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int id,
                                                                                                     int userId,
                                                                                                     CancellationToken cancellationToken)
    {
        List<TrackingLogEntry> entries = 
            await trackingRepository.DeleteTrackingLogEntryById(id, userId, cancellationToken);
        return entries.Select(e => e.ToDto()).ToList();
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<ServiceResponse<TrackingLogEntryStatusGetResponse>> CreateTrackingLogStatus(
        TrackingLogEntryStatusCreateRequest request,
        int userId,
        CancellationToken cancellationToken)
    {
        TrackingLogEntity? trackingLog = await trackingRepository.GetTrackingLogEntityById(request.TrackingLogId,
                                                                                           cancellationToken);

        if (trackingLog is null)
        {
            return new ServiceResponse<TrackingLogEntryStatusGetResponse>(actionResultType: ActionResultType.UserError,
                                                                          message: MessageResources.CouldNotCreateMessage);
        }

        if (!CanEditTrackingEntity(trackingLog, userId))
        {
            return new ServiceResponse<TrackingLogEntryStatusGetResponse>(actionResultType: ActionResultType.Unauthorized,
                                                                          message: MessageResources.AccessDeniedMessage);
        }
        
        NewTrackingLogEntryStatus newStatus = new(StringAttribute.CreateRequired(request.Title),
                                                  StringAttribute.CreateOptional(request.Description),
                                                  request.TrackingLogId,
                                                  userId,
                                                  dateTimeService.UtcNow);

        TrackingLogEntryStatus? status = await trackingRepository.CreateTrackingLogEntryStatus(newStatus, cancellationToken);
        
        if (status is null)
        {
            return new ServiceResponse<TrackingLogEntryStatusGetResponse>(actionResultType: ActionResultType.ServerError,
                                                                          message: MessageResources.CouldNotCreateMessage);
        }

        return status.ToDto();
    }

    public async Task<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>> DeleteTrackingLogStatus(int id,
                                                                                                        int userId,
                                                                                                        CancellationToken cancellationToken)
    {
        List<TrackingLogEntryStatus> statuses = 
            await trackingRepository.DeleteTrackingLogEntryStatusById(id, cancellationToken);
        return statuses.Select(s => s.ToDto()).ToList();
    }

    #endregion
}