#region Usings

using TaskManagerBackend.Common;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Repositories.Tracking;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Services.Tracking;

public class TrackingService : ITrackingService
{
    private readonly ITrackingRepository _trackingRepository;
    private readonly IDateTimeService _dateTimeService;
        
    private const string CouldNotCreateMessage = "Could not create resource";
    private const string ResourceDoesNotExist = "Resource does not exist";

    public TrackingService(ITrackingRepository trackingRepository,
                           IDateTimeService dateTimeService)
    {
        _trackingRepository = trackingRepository;
        _dateTimeService = dateTimeService;
    }
    
    #region Tracking Logs
    
    public async Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(int userId, TrackingLogCreateRequest newLog)
    {
        NewTrackingLog logToInsert = new()
                                     {
                                         Title = newLog.Title,
                                         Description = newLog.Description,
                                         CreatedById = userId,
                                         CreatedAt = _dateTimeService.UtcNow
                                     };

        ServiceResponse<TrackingLogGetResponse> response = new()
                                                           {
                                                               Data = await _trackingRepository.InsertTrackingLog(logToInsert)
                                                           };

        if (response.Data == null)
        {
            response.Success = false;
            response.Message = CouldNotCreateMessage;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogs(int userId)
    {
        ServiceResponse<List<TrackingLogGetResponse>> response = new()
                                                              {
                                                                  Data = await _trackingRepository.GetAllTrackingLogs(userId)
                                                              };

        return response;
    }

    public async Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id)
    {
        ServiceResponse<TrackingLogGetResponse> response = new()
                                                        {
                                                            Data = await _trackingRepository.GetTrackingLogById(id)
                                                        };
            
        if (response.Data == null)
        {
            response.Success = false;
            response.Message = ResourceDoesNotExist;
        }

        return response;
    }

    public async Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int userId, int boardId)
    {
        ServiceResponse<List<TrackingLogGetResponse>> response = new()
                                                              {
                                                                  Data = await _trackingRepository.DeleteTrackingLogById(userId, boardId)
                                                              };

        return response;
    }

    #endregion

    #region Tracking Log Entry Statuses

    public async Task<ServiceResponse<TrackingLogEntryStatus>> CreateTrackingLogStatus(int userId, TrackingLogEntryStatusCreateRequest newStatus)
    {
        NewTrackingLogEntryStatus statusToInsert = new()
                                     {
                                         Title = newStatus.Title,
                                         Description = newStatus.Description,
                                         TrackingLogId = newStatus.TrackingLogId,
                                         CreatedById = userId,
                                         CreatedAt = _dateTimeService.UtcNow
                                     };

        ServiceResponse<TrackingLogEntryStatus> response = new()
                                                           {
                                                               Data = await _trackingRepository.InsertTrackingLogEntryStatus(statusToInsert)
                                                           };

        if (response.Data == null)
        {
            response.Success = false;
            response.Message = CouldNotCreateMessage;
        }

        return response;
    }

    #endregion
}