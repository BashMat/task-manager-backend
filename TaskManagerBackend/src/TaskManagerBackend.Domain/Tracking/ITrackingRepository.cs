#region Usings

using TaskManagerBackend.Domain.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Domain.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    // TODO: Perhaps nullability is excessive
    Task<TrackingLog.TrackingLog?> CreateTrackingLog(NewTrackingLog logToInsert);
    Task<List<TrackingLog.TrackingLog>> GetAllTrackingLogs(int userId);
    Task<TrackingLog.TrackingLog?> GetTrackingLogById(int trackingLogId);
    Task<List<TrackingLog.TrackingLog>> DeleteTrackingLogById(int userId, int trackingLogId);

    #endregion
    
    #region Tracking Log Entries

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntry.TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry logEntryToInsert);
    Task<List<TrackingLogEntry.TrackingLogEntry>> GetAllTrackingLogEntries(int userId);
    Task<TrackingLogEntry.TrackingLogEntry?> GetTrackingLogEntryById(int trackingLogEntryId);
    Task<TrackingLogEntry.TrackingLogEntry?> UpdateTrackingLogEntryById(int trackingLogEntryId, 
                                                                        UpdatableTrackingLogEntry updatableTrackingLogEntry);
    Task<List<TrackingLogEntry.TrackingLogEntry>> DeleteTrackingLogEntryById(int userId, int trackingLogEntryId);

    #endregion

    #region Tracking Log Entry Statuses

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntryStatus.TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert);
    Task<List<TrackingLogEntryStatus.TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId);

    #endregion
}