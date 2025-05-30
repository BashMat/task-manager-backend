using TaskManagerBackend.Dto.Tracking.TrackingLog;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntry;
using TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

namespace TaskManagerBackend.Domain.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    Task<TrackingLogGetResponse?> InsertTrackingLog(NewTrackingLog logToInsert);
    Task<List<TrackingLogGetResponse>> GetAllTrackingLogs(int userId);
    Task<TrackingLogGetResponse?> GetTrackingLogById(int trackingLogId);
    Task<List<TrackingLogGetResponse>> DeleteTrackingLogById(int userId, int trackingLogId);

    #endregion
    
    #region Tracking Log Entries

    Task<TrackingLogEntryGetResponse?> InsertTrackingLogEntry(NewTrackingLogEntry logEntryToInsert);
    Task<List<TrackingLogEntryGetResponse>> GetAllTrackingLogEntries(int userId);
    Task<TrackingLogEntryGetResponse?> GetTrackingLogEntryById(int trackingLogEntryId);
    Task<TrackingLogEntryGetResponse?> UpdateTrackingLogEntryById(int id, 
                                                                  UpdatableTrackingLogEntry updatableTrackingLogEntry);
    Task<List<TrackingLogEntryGetResponse>> DeleteTrackingLogEntryById(int userId, int trackingLogEntryId);

    #endregion

    #region Tracking Log Entry Statuses

    Task<TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert);

    #endregion
}