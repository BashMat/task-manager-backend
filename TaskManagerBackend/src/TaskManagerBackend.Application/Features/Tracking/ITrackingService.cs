#region Usings

using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;
using TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;
using TaskManagerBackend.Application.Utility;

#endregion

namespace TaskManagerBackend.Application.Features.Tracking;

public interface ITrackingService
{
    #region Tracking Logs

    Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(int userId, TrackingLogCreateRequest newLog);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogsByUserId(int userId);
    Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int userId, int trackingLogId);

    #endregion
    
    #region Tracking Log Entries

    Task<ServiceResponse<TrackingLogEntryGetResponse>> CreateTrackingLogEntry(int userId,
                                                                              TrackingLogEntryCreateRequest newLog);
    Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> GetAllTrackingLogEntriesByUserId(int userId);
    Task<ServiceResponse<TrackingLogEntryGetResponse>> GetTrackingLogEntryById(int id);
    Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntry(int userId,
                                                                              int id,
                                                                              UpdateTrackingLogEntryRequest request);
    Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int userId,
                                                                                        int trackingLogEntryId);

    #endregion

    #region Tracking Log Entry Statuses

    Task<ServiceResponse<TrackingLogEntryStatusGetResponse>> CreateTrackingLogStatus(int userId, TrackingLogEntryStatusCreateRequest newStatus);
    Task<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>> DeleteTrackingLogStatus(int userId,
                                                                                int trackingLogEntryStatusId);

    #endregion
}