using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    Task<TrackingLogGetResponse?> InsertTrackingLog(NewTrackingLog logToInsert);
    Task<List<TrackingLogGetResponse>> GetAllTrackingLogs(int userId);
    Task<TrackingLogGetResponse?> GetTrackingLogById(int trackingLogId);
    Task<List<TrackingLogGetResponse>> DeleteTrackingLogById(int userId, int trackingLogId);

    #endregion

    #region Tracking Log Entry Statuses

    Task<TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert);

    #endregion
}