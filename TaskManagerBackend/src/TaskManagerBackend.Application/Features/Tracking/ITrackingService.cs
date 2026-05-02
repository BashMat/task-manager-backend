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

    Task<ServiceResponse<TrackingLogGetResponse>> CreateTrackingLog(TrackingLogCreateRequest request,
                                                                    int userId,
                                                                    CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> GetAllTrackingLogsByUserId(int userId,
                                                                                   CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogGetResponse>> GetTrackingLogById(int id,
                                                                     int userId,
                                                                     CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogGetResponse>> EditTrackingLog(TrackingLogEditRequest request,
                                                                  int userId,
                                                                  CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogGetResponse>>> DeleteTrackingLogById(int id,
                                                                              int userId,
                                                                              CancellationToken cancellationToken);

    #endregion
    
    #region Tracking Log Entries

    Task<ServiceResponse<TrackingLogEntryGetResponse>> CreateTrackingLogEntry(TrackingLogEntryCreateRequest request,
                                                                              int userId,
                                                                              CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> GetAllTrackingLogEntriesByUserId(int userId,
                                                                                              CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogEntryGetResponse>> GetTrackingLogEntryById(int id,
                                                                               int userId,
                                                                               CancellationToken cancellationToken);
    Task<ServiceResponse<TrackingLogEntryGetResponse>> UpdateTrackingLogEntryById(int id,
                                                                                  UpdateTrackingLogEntryRequest request,
                                                                                  int userId,
                                                                                  CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogEntryGetResponse>>> DeleteTrackingLogEntryById(int id,
                                                                                        int userId,
                                                                                        CancellationToken cancellationToken);

    #endregion

    #region Tracking Log Entry Statuses

    Task<ServiceResponse<TrackingLogEntryStatusGetResponse>> CreateTrackingLogStatus(TrackingLogEntryStatusCreateRequest request,
                                                                                     int userId,
                                                                                     CancellationToken cancellationToken);
    Task<ServiceResponse<List<TrackingLogEntryStatusGetResponse>>> DeleteTrackingLogStatus(int id,
                                                                                           int userId,
                                                                                           CancellationToken cancellationToken);

    #endregion
}