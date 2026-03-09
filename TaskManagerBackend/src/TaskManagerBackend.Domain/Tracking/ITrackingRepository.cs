namespace TaskManagerBackend.Domain.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    // TODO: Perhaps nullability is excessive
    Task<TrackingLog?> CreateTrackingLog(NewTrackingLog logToInsert);
    Task<List<TrackingLog>> GetAllTrackingLogs(int userId);
    Task<TrackingLog?> GetTrackingLogById(int trackingLogId);
    Task<List<TrackingLog>> DeleteTrackingLogById(int userId, int trackingLogId);

    #endregion
    
    #region Tracking Log Entries

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry logEntryToInsert);
    Task<List<TrackingLogEntry>> GetAllTrackingLogEntries(int userId);
    Task<TrackingLogEntry?> GetTrackingLogEntryById(int trackingLogEntryId);
    Task<TrackingLogEntry?> UpdateTrackingLogEntryById(int trackingLogEntryId, 
                                                       UpdatableTrackingLogEntry updatableTrackingLogEntry);
    Task<List<TrackingLogEntry>> DeleteTrackingLogEntryById(int userId, int trackingLogEntryId);

    #endregion

    #region Tracking Log Entry Statuses

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert);
    Task<List<TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId);

    #endregion
}