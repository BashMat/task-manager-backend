#region Usings

using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Entities;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Domain.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;
using TaskManagerBackend.Domain.Workflow;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking;

public class TrackingService(ITrackingRepository trackingRepository,
                             IDateTimeService dateTimeService) : ITrackingService
{
    #region Tracking Logs

    public async Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(int userId,
                                                                                 TrackingLogCreateRequest request,
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

    public async Task<ServiceResponse<TrackingLogGetResponse>> EditTrackingLog(int userId,
                                                                               TrackingLogEditRequest request,
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

    private static bool CanEditTrackingEntity(IAuditedEntity trackingEntity, int userId)
    {
        return trackingEntity.CreatedBy == userId;
    }
    
    private async Task<ServiceResponse<T>?> CanEditTrackingLog<T>(int trackingLogId,
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

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int userId, 
                                                                                           int trackingLogId, 
                                                                                           CancellationToken cancellationToken)
    {
        List<TrackingLog> logs = await trackingRepository.DeleteTrackingLogById(userId, trackingLogId, cancellationToken);
        return logs.Select(l => l.ToDto()).ToList();
    }

    #endregion

    #region Tracking Log Entries

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> CreateTrackingLogEntry(int userId, 
                                                                                           TrackingLogEntryCreateRequest request, 
                                                                                           CancellationToken cancellationToken)
    {
        ServiceResponse<TrackingLogEntryGetResponse>? failedResultOrNull = 
            await CanEditTrackingLog<TrackingLogEntryGetResponse>(request.TrackingLogId, userId, cancellationToken);

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

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntry(int userId,
                                                                                           int id,
                                                                                           UpdateTrackingLogEntryRequest request,
                                                                                           CancellationToken cancellationToken)
    {
        TrackingLogEntry? entry = await trackingRepository.GetTrackingLogEntryById(id, cancellationToken);

        if (entry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.ResourceNotFound,
                                                                    message: MessageResources.ResourceDoesNotExist);
        }

        // TODO: when resource is created and updated locally immediately,
        // DateTime value is considered equal.
        // Perhaps some sort of more accurate timestamp should be used
        if (entry.UpdatedAt > request.UpdatedAt)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResultType: ActionResultType.DataConflict,
                                                                    message: MessageResources.UpdateConflict);
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

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int userId, 
                                                                                                     int trackingLogEntryId, 
                                                                                                     CancellationToken cancellationToken)
    {
        List<TrackingLogEntry> entries = 
            await trackingRepository.DeleteTrackingLogEntryById(userId, trackingLogEntryId, cancellationToken);
        return entries.Select(e => e.ToDto()).ToList();
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<ServiceResponse<TrackingLogEntryStatusGetResponse>> CreateTrackingLogStatus(int userId, 
                                                                                                  TrackingLogEntryStatusCreateRequest request,
                                                                                                  CancellationToken cancellationToken)
    {
        NewTrackingLogEntryStatus statusToInsert = new(StringAttribute.CreateRequired(request.Title),
                                                       StringAttribute.CreateOptional(request.Description),
                                                       request.TrackingLogId,
                                                       userId,
                                                       dateTimeService.UtcNow);

        TrackingLogEntryStatus? status = await trackingRepository.CreateTrackingLogEntryStatus(statusToInsert, cancellationToken);
        TrackingLogEntryStatusGetResponse? response = status?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogEntryStatusGetResponse>(actionResultType: ActionResultType.ServerError,
                                                                          message: MessageResources.CouldNotCreateMessage);
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>> DeleteTrackingLogStatus(int userId, 
                                                                                                        int trackingLogEntryStatusId, 
                                                                                                        CancellationToken cancellationToken)
    {
        List<TrackingLogEntryStatus> statuses = 
            await trackingRepository.DeleteTrackingLogEntryStatusById(trackingLogEntryStatusId, cancellationToken);
        return statuses.Select(s => s.ToDto()).ToList();
    }

    #endregion
}