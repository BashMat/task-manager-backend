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

    Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(int userId,
                                                                    TrackingLogCreateRequest request,
                                                                    CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogsByUserId(int userId,
                                                                                   CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id, CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogGetResponse>> EditTrackingLog(int userId,
                                                                  TrackingLogEditRequest request,
                                                                  CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int userId,
                                                                              int trackingLogId,
                                                                              CancellationToken cancellationToken);

    #endregion
    
    #region Tracking Log Entries

    Task<ServiceResponse<TrackingLogEntryGetResponse>> CreateTrackingLogEntry(int userId,
                                                                              TrackingLogEntryCreateRequest request,
                                                                              CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> GetAllTrackingLogEntriesByUserId(int userId,
                                                                                              CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogEntryGetResponse>> GetTrackingLogEntryById(int id,
                                                                               CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntry(int userId,
                                                                              int id,
                                                                              UpdateTrackingLogEntryRequest request,
                                                                              CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int userId, 
                                                                                        int trackingLogEntryId,
                                                                                        CancellationToken cancellationToken);

    #endregion

    #region Tracking Log Entry Statuses

    Task<ServiceResponse<TrackingLogEntryStatusGetResponse>> CreateTrackingLogStatus(int userId,
                                                                                     TrackingLogEntryStatusCreateRequest request,
                                                                                     CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>> DeleteTrackingLogStatus(int userId, 
                                                                                           int trackingLogEntryStatusId,
                                                                                           CancellationToken cancellationToken);

    #endregion
}