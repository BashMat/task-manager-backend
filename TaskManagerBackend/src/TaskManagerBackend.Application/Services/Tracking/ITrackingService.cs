#region Usings

using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Application.Services.Tracking;

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
    Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int userId,
                                                                                        int trackingLogEntryId);

    #endregion

    #region Tracking Log Entry Statuses

    Task<ServiceResponse<TrackingLogEntryStatus>> CreateTrackingLogStatus(int userId, TrackingLogEntryStatusCreateRequest newStatus);

    #endregion
}