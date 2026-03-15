#region Usings

using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Domain.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking;

public class TrackingService(ITrackingRepository trackingRepository,
                             IDateTimeService dateTimeService) : ITrackingService
{
    private const string CouldNotCreateMessage = "Could not create resource";
    private const string ResourceDoesNotExist = "Resource does not exist";
    private const string UpdateConflict = "Resource was updated";

    #region Tracking Logs

    public async Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(int userId,
                                                                                 TrackingLogCreateRequest request,
                                                                                 CancellationToken cancellationToken)
    {
        NewTrackingLog logToInsert = new(StringAttribute.CreateRequired(request.Title),
                                         StringAttribute.CreateOptional(request.Description),
                                         userId,
                                         dateTimeService.UtcNow);

        TrackingLog? log = await trackingRepository.CreateTrackingLog(logToInsert, cancellationToken);
        TrackingLogGetResponse? response = log?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResult: ActionResults.ServerError,
                                                               message: CouldNotCreateMessage);
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogsByUserId(int userId,
                                                                                                CancellationToken cancellationToken)
    {
        List<TrackingLog> logs = await trackingRepository.GetAllTrackingLogs(userId, cancellationToken);
        return logs.Select(l => l.ToDto()).ToList();
    }

    public async Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id,
                                                                                  CancellationToken cancellationToken)
    {
        TrackingLog? log = await trackingRepository.GetTrackingLogById(id, cancellationToken);
        TrackingLogGetResponse? response = log?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResult: ActionResults.ResourceNotFound,
                                                               message: ResourceDoesNotExist);
        }

        return response;
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
        NewTrackingLogEntry logEntryToInsert = new(StringAttribute.CreateRequired(request.Title),
                                                   StringAttribute.CreateOptional(request.Description),
                                                   request.TrackingLogId,
                                                   request.StatusId,
                                                   request.Priority,
                                                   request.OrderIndex,
                                                   userId,
                                                   dateTimeService.UtcNow);
        
        TrackingLogEntry? entry = await trackingRepository.CreateTrackingLogEntry(logEntryToInsert, cancellationToken);
        TrackingLogEntryGetResponse? response = entry?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.ServerError,
                                                                    message: CouldNotCreateMessage);
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> GetAllTrackingLogEntriesByUserId(int userId, 
                                                                                                           CancellationToken cancellationToken)
    {
        List<TrackingLogEntry> entries = await trackingRepository.GetAllTrackingLogEntries(userId, cancellationToken);
        return entries.Select(e => e.ToDto()).ToList();
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> GetTrackingLogEntryById(int id,
                                                                                            CancellationToken cancellationToken)
    {
        TrackingLogEntry? entry = await trackingRepository.GetTrackingLogEntryById(id, cancellationToken);
        TrackingLogEntryGetResponse? response = entry?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.ResourceNotFound,
                                                                    message: ResourceDoesNotExist);
        }

        return response;
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntry(int userId,
                                                                                           int id,
                                                                                           UpdateTrackingLogEntryRequest request,
                                                                                           CancellationToken cancellationToken)
    {
        TrackingLogEntry? entry = await trackingRepository.GetTrackingLogEntryById(id, cancellationToken);

        if (entry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.ResourceNotFound,
                                                                    message: ResourceDoesNotExist);
        }

        // TODO: when resource is created and updated locally immediately,
        // DateTime value is considered equal.
        // Perhaps some sort of more accurate timestamp should be used
        if (entry.UpdatedAt > request.UpdatedAt)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.DataConflict,
                                                                    message: UpdateConflict);
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
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.ResourceNotFound,
                                                                    message: ResourceDoesNotExist);
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
            return new ServiceResponse<TrackingLogEntryStatusGetResponse>(actionResult: ActionResults.ServerError,
                                                                          message: CouldNotCreateMessage);
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