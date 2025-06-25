namespace TaskManagerBackend.Domain.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    // TODO: Perhaps nullability is excessive
    Task<TrackingLog?> InsertTrackingLog(NewTrackingLog logToInsert);
    Task<List<TrackingLog>> GetAllTrackingLogs(int userId);
    Task<TrackingLog?> GetTrackingLogById(int trackingLogId);
    Task<List<TrackingLog>> DeleteTrackingLogById(int userId, int trackingLogId);

    #endregion
    
    #region Tracking Log Entries

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntry?> InsertTrackingLogEntry(NewTrackingLogEntry logEntryToInsert);
    Task<List<TrackingLogEntry>> GetAllTrackingLogEntries(int userId);
    Task<TrackingLogEntry?> GetTrackingLogEntryById(int trackingLogEntryId);
    Task<TrackingLogEntry?> UpdateTrackingLogEntryById(int id, 
                                                                  UpdatableTrackingLogEntry updatableTrackingLogEntry);
    Task<List<TrackingLogEntry>> DeleteTrackingLogEntryById(int userId, int trackingLogEntryId);

    #endregion

    #region Tracking Log Entry Statuses

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntryStatus?> InsertTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert);
    Task<List<TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId);

    #endregion
}