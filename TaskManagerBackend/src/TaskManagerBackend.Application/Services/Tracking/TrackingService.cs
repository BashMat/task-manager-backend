#region Usings

using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Services.Tracking;

public class TrackingService : ITrackingService
{
    private readonly ITrackingRepository _trackingRepository;
    private readonly IDateTimeService _dateTimeService;

    private const string CouldNotCreateMessage = "Could not create resource";
    private const string ResourceDoesNotExist = "Resource does not exist";
    private const string UpdateConflict = "Resource was updated";

    public TrackingService(ITrackingRepository trackingRepository,
                           IDateTimeService dateTimeService)
    {
        _trackingRepository = trackingRepository;
        _dateTimeService = dateTimeService;
    }

    #region Tracking Logs

    public async Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(int userId,
                                                                                 TrackingLogCreateRequest newLog)
    {
        NewTrackingLog logToInsert = new()
                                     {
                                         Title = newLog.Title,
                                         Description = newLog.Description,
                                         CreatedById = userId,
                                         CreatedAt = _dateTimeService.UtcNow
                                     };

        TrackingLog? log = await _trackingRepository.InsertTrackingLog(logToInsert);
        ServiceResponse<TrackingLogGetResponse> response = log?.ToDto();

        if (response.Data == null)
        {
            response.Success = false;
            response.Message = CouldNotCreateMessage;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogsByUserId(int userId)
    {
        List<TrackingLog> logs = await _trackingRepository.GetAllTrackingLogs(userId);
        ServiceResponse<List<TrackingLogGetResponse>> response = logs.Select(log => log.ToDto()).ToList();

        return response;
    }

    public async Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id)
    {
        TrackingLog? log = await _trackingRepository.GetTrackingLogById(id);
        ServiceResponse<TrackingLogGetResponse> response = log?.ToDto();

        if (response.Data == null)
        {
            response.Success = false;
            response.Message = ResourceDoesNotExist;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int userId, int trackingLogId)
    {
        List<TrackingLog> logs = await _trackingRepository.DeleteTrackingLogById(userId, trackingLogId);
        ServiceResponse<List<TrackingLogGetResponse>> response = logs.Select(log => log.ToDto()).ToList();

        return response;
    }

    #endregion

    #region Tracking Log Entries

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> CreateTrackingLogEntry(int userId,
                                                                                           TrackingLogEntryCreateRequest newLogEntry)
    {
        NewTrackingLogEntry logEntryToInsert = new()
                                               {
                                                   Title = newLogEntry.Title,
                                                   Description = newLogEntry.Description,
                                                   TrackingLogId = newLogEntry.TrackingLogId,
                                                   StatusId = newLogEntry.StatusId,
                                                   Priority = newLogEntry.Priority,
                                                   OrderIndex = newLogEntry.OrderIndex,
                                                   CreatedById = userId,
                                                   CreatedAt = _dateTimeService.UtcNow
                                               };
        
        TrackingLogEntry? entry = await _trackingRepository.InsertTrackingLogEntry(logEntryToInsert);
        ServiceResponse<TrackingLogEntryGetResponse> response = entry?.ToDto();

        if (response.Data == null)
        {
            response.Success = false;
            response.Message = CouldNotCreateMessage;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> GetAllTrackingLogEntriesByUserId(int userId)
    {
        List<TrackingLogEntry> entries = await _trackingRepository.GetAllTrackingLogEntries(userId);
        ServiceResponse<List<TrackingLogEntryGetResponse>> response = entries.Select(log => log.ToDto()).ToList();

        return response;
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> GetTrackingLogEntryById(int id)
    {
        TrackingLogEntry? entry = await _trackingRepository.GetTrackingLogEntryById(id);
        ServiceResponse<TrackingLogEntryGetResponse> response = entry?.ToDto();

        if (response.Data is null)
        {
            response.Success = false;
            response.Message = ResourceDoesNotExist;
        }

        return response;
    }

    public async Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntry(int userId,
                                                                                           int id, 
                                                                                           UpdateTrackingLogEntryRequest request)
    {
        TrackingLogEntry? entry = await _trackingRepository.GetTrackingLogEntryById(id);
        ServiceResponse<TrackingLogEntryGetResponse> response = new();

        if (entry is null)
        {
            response.Success = false;
            response.Message = ResourceDoesNotExist;
            return response;
        }

        // TODO: when resource is created and updated locally immediately,
        // DateTime value is considered equal.
        // Perhaps some sort of more accurate timestamp should be used
        if (entry.UpdatedAt > request.UpdatedAt)
        {
            response.Success = false;
            response.Message = UpdateConflict;
            return response;
        }

        UpdatableTrackingLogEntry updatableTrackingLogEntry = new()
                                                              {
                                                                  Title = request.Title,
                                                                  Description = request.Description,
                                                                  TrackingLogId = request.TrackingLogId,
                                                                  StatusId = request.StatusId,
                                                                  Priority = request.Priority,
                                                                  OrderIndex = request.OrderIndex,
                                                              };

        updatableTrackingLogEntry.SetUpdatedData(userId,
                                                 _dateTimeService);

        TrackingLogEntry? updatedEntry = 
            await _trackingRepository.UpdateTrackingLogEntryById(id, updatableTrackingLogEntry);
        
        if (updatedEntry is null)
        {
            response.Success = false;
            response.Message = ResourceDoesNotExist;
            return response;
        }

        response.Data = updatedEntry.ToDto();
        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int userId, 
                                                                                                     int trackingLogEntryId)
    {
        List<TrackingLogEntry> entries = await _trackingRepository.DeleteTrackingLogEntryById(userId, trackingLogEntryId);
        ServiceResponse<List<TrackingLogEntryGetResponse>> response = entries.Select(log => log.ToDto()).ToList();

        return response;
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<ServiceResponse<TrackingLogEntryStatusGetResponse>> CreateTrackingLogStatus(int userId,
                                                                                       TrackingLogEntryStatusCreateRequest newStatus)
    {
        NewTrackingLogEntryStatus statusToInsert = new()
                                                   {
                                                       Title = newStatus.Title,
                                                       Description = newStatus.Description,
                                                       TrackingLogId = newStatus.TrackingLogId,
                                                       CreatedById = userId,
                                                       CreatedAt = _dateTimeService.UtcNow
                                                   };

        TrackingLogEntryStatus? status = await _trackingRepository.InsertTrackingLogEntryStatus(statusToInsert);
        ServiceResponse<TrackingLogEntryStatusGetResponse> response = status?.ToDto();

        if (response.Data == null)
        {
            response.Success = false;
            response.Message = CouldNotCreateMessage;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>> DeleteTrackingLogStatus(int userId, 
                                                                                             int trackingLogEntryStatusId)
    {
        List<TrackingLogEntryStatus> statuses = await _trackingRepository.DeleteTrackingLogEntryStatusById(userId);
        ServiceResponse<List<TrackingLogEntryStatusGetResponse>> response = statuses.Select(log => log.ToDto()).ToList();

        return response;
    }

    #endregion
}