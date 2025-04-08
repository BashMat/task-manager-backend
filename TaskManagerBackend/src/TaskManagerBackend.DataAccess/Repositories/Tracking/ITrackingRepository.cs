using TaskManagerBackend.Dto.Tracking.TrackingLog;

namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    Task<TrackingLogGetResponse?> Insert(NewTrackingLog logToInsert);
    Task<List<TrackingLogGetResponse>> GetAll(int userId);
    Task<TrackingLogGetResponse?> GetById(int trackingLogId);
    Task<List<TrackingLogGetResponse>> Delete(int userId, int trackingLogId);

    #endregion
}