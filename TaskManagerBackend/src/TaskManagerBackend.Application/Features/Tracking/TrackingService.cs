#region Usings

using TaskManagerBackend.Application.Features.History;
using TaskManagerBackend.Application.Features.History.Dtos;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain;
using TaskManagerBackend.Domain.Entities;
using TaskManagerBackend.Domain.Tracking;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking;

public class TrackingService : ITrackingService
{
    private readonly ITrackingRepository _trackingRepository;
    private readonly IHistoryService _historyService;
    private readonly IDateTimeService _dateTimeService;

    private const string CouldNotCreateMessage = "Could not create resource";
    private const string ResourceDoesNotExist = "Resource does not exist";
    private const string UpdateConflict = "Resource was updated";

    public TrackingService(ITrackingRepository trackingRepository,
                           IHistoryService historyService,
                           IDateTimeService dateTimeService)
    {
        _trackingRepository = trackingRepository;
        _historyService = historyService;
        _dateTimeService = dateTimeService;
    }

    #region Tracking Logs

    public async Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(int userId,
                                                                                 TrackingLogCreateRequest newLog)
    {
        NewTrackingLog logToInsert = new(newLog.Title,
                                         newLog.Description,
                                         userId,
                                         _dateTimeService);

        TrackingLog? log = await _trackingRepository.InsertTrackingLog(logToInsert);
        TrackingLogGetResponse? response = log?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResult: ActionResults.ServerError,
                                                               message: CouldNotCreateMessage);
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogsByUserId(int userId)
    {
        List<TrackingLog> logs = await _trackingRepository.GetAllTrackingLogs(userId);
        return logs.Select(l => l.ToDto()).ToList();
    }

    public async Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id)
    {
        TrackingLog? log = await _trackingRepository.GetTrackingLogById(id);
        TrackingLogGetResponse? response = log?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogGetResponse>(actionResult: ActionResults.ResourceNotFound,
                                                               message: ResourceDoesNotExist);
        }

        return response;
    }
    
    public async Task<ServiceResponse<GetEntityHistoryResponse>> GetTrackingLogHistoryById(int id)
    {
        return await _historyService.GetEntityHistory(EntityType.TrackingLog, id);
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int userId,
                                                                                           int trackingLogId)
    {
        List<TrackingLog> logs = await _trackingRepository.DeleteTrackingLogById(userId, trackingLogId);
        return logs.Select(l => l.ToDto()).ToList();
    }

    #endregion

    #region Tracking Log Entries

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> CreateTrackingLogEntry(int userId,
                                                                                           TrackingLogEntryCreateRequest newLogEntry)
    {
        NewTrackingLogEntry logEntryToInsert = new(newLogEntry.Title,
                                                   newLogEntry.Description,
                                                   newLogEntry.TrackingLogId, 
                                                   newLogEntry.StatusId,
                                                   newLogEntry.Priority, 
                                                   (decimal)newLogEntry.OrderIndex, 
                                                   userId,
                                                   _dateTimeService);
        
        TrackingLogEntry? entry = await _trackingRepository.InsertTrackingLogEntry(logEntryToInsert);
        TrackingLogEntryGetResponse? response = entry?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.ServerError,
                                                                    message: CouldNotCreateMessage);
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> GetAllTrackingLogEntriesByUserId(int userId)
    {
        List<TrackingLogEntry> entries = await _trackingRepository.GetAllTrackingLogEntries(userId);
        return entries.Select(e => e.ToDto()).ToList();
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> GetTrackingLogEntryById(int id)
    {
        TrackingLogEntry? entry = await _trackingRepository.GetTrackingLogEntryById(id);
        TrackingLogEntryGetResponse? response = entry?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.ResourceNotFound,
                                                                    message: ResourceDoesNotExist);
        }

        return response;
    }
    
    public async Task<ServiceResponse<GetEntityHistoryResponse>> GetTrackingLogEntryHistoryById(int id)
    {
        return await _historyService.GetEntityHistory(EntityType.TrackingLogEntry, id);
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntry(int userId,
                                                                                           int id, 
                                                                                           UpdateTrackingLogEntryRequest request)
    {
        TrackingLogEntry? entry = await _trackingRepository.GetTrackingLogEntryById(id);

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

        UpdatableTrackingLogEntry updatableTrackingLogEntry = new(request.Title,
                                                                  request.Description,
                                                                  request.TrackingLogId,
                                                                  request.StatusId,
                                                                  request.Priority,
                                                                  (decimal)request.OrderIndex,
                                                                  userId,
                                                                  _dateTimeService);

        TrackingLogEntry? updatedEntry = 
            await _trackingRepository.UpdateTrackingLogEntryById(id, updatableTrackingLogEntry);
        
        if (updatedEntry is null)
        {
            return new ServiceResponse<TrackingLogEntryGetResponse>(actionResult: ActionResults.ResourceNotFound,
                                                                    message: ResourceDoesNotExist);
        }
        
        return updatedEntry.ToDto();
    }

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int userId, 
                                                                                                     int trackingLogEntryId)
    {
        List<TrackingLogEntry> entries = 
            await _trackingRepository.DeleteTrackingLogEntryById(userId, trackingLogEntryId);
        return entries.Select(e => e.ToDto()).ToList();
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<ServiceResponse<TrackingLogEntryStatusGetResponse>> CreateTrackingLogStatus(int userId, 
                                                                                                  TrackingLogEntryStatusCreateRequest newStatus)
    {
        NewTrackingLogEntryStatus statusToInsert = new(newStatus.Title,
                                                       newStatus.Description,
                                                       newStatus.TrackingLogId,
                                                       userId,
                                                       _dateTimeService);

        TrackingLogEntryStatus? status = await _trackingRepository.InsertTrackingLogEntryStatus(statusToInsert);
        TrackingLogEntryStatusGetResponse? response = status?.ToDto();

        if (response is null)
        {
            return new ServiceResponse<TrackingLogEntryStatusGetResponse>(actionResult: ActionResults.ServerError,
                                                                          message: CouldNotCreateMessage);
        }

        return response;
    }
    public async Task<ServiceResponse<GetEntityHistoryResponse>> GetTrackingLogEntryStatusHistoryById(int id)
    {
        return await _historyService.GetEntityHistory(EntityType.TrackingLogEntryStatus, id);
    }

    public async Task<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>> DeleteTrackingLogStatus(int userId, 
                                                                                                        int trackingLogEntryStatusId)
    {
        List<TrackingLogEntryStatus> statuses = 
            await _trackingRepository.DeleteTrackingLogEntryStatusById(trackingLogEntryStatusId);
        return statuses.Select(s => s.ToDto()).ToList();
    }

    #endregion
}