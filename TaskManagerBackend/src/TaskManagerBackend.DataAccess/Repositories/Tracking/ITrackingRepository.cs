using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    Task<TrackingLogGetResponse?> Insert(NewTrackingLog logToInsert);
    Task<List<TrackingLogGetResponse>> GetAll(int userId);
    Task<TrackingLogGetResponse?> GetById(int trackingLogId);
    Task<List<TrackingLogGetResponse>> Delete(int userId, int trackingLogId);

    #endregion

    #region Tracking Log Entry Statuses

    Task<TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert);

    #endregion
}