using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

namespace TaskManagerBackend.Domain.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogGetResponse?> InsertTrackingLog(NewTrackingLog logToInsert);
    Task<List<TrackingLogGetResponse>> GetAllTrackingLogs(int userId);
    Task<TrackingLogGetResponse?> GetTrackingLogById(int trackingLogId);
    Task<List<TrackingLogGetResponse>> DeleteTrackingLogById(int userId, int trackingLogId);

    #endregion
    
    #region Tracking Log Entries

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntryGetResponse?> InsertTrackingLogEntry(NewTrackingLogEntry logEntryToInsert);
    Task<List<TrackingLogEntryGetResponse>> GetAllTrackingLogEntries(int userId);
    Task<TrackingLogEntryGetResponse?> GetTrackingLogEntryById(int trackingLogEntryId);
    Task<TrackingLogEntryGetResponse?> UpdateTrackingLogEntryById(int id, 
                                                                  UpdatableTrackingLogEntry updatableTrackingLogEntry);
    Task<List<TrackingLogEntryGetResponse>> DeleteTrackingLogEntryById(int userId, int trackingLogEntryId);

    #endregion

    #region Tracking Log Entry Statuses

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert);
    Task<List<TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId);

    #endregion
}