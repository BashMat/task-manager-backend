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
    Task<TrackingLog.TrackingLog?> CreateTrackingLog(NewTrackingLog logToInsert, CancellationToken cancellationToken);
    Task<List<TrackingLog.TrackingLog>> GetAllTrackingLogs(int userId, CancellationToken cancellationToken);
    Task<TrackingLog.TrackingLog?> GetTrackingLogById(int trackingLogId, CancellationToken cancellationToken);
    Task<List<TrackingLog.TrackingLog>> DeleteTrackingLogById(int userId,
                                                              int trackingLogId,
                                                              CancellationToken cancellationToken);

    #endregion
    
    #region Tracking Log Entries

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntry.TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry logEntryToInsert,
                                                                    CancellationToken cancellationToken);
    Task<List<TrackingLogEntry.TrackingLogEntry>> GetAllTrackingLogEntries(int userId, 
                                                                           CancellationToken cancellationToken);
    Task<TrackingLogEntry.TrackingLogEntry?> GetTrackingLogEntryById(int trackingLogEntryId,
                                                                     CancellationToken cancellationToken);
    Task<TrackingLogEntry.TrackingLogEntry?> UpdateTrackingLogEntryById(int trackingLogEntryId,
                                                                        UpdatableTrackingLogEntry updatableTrackingLogEntry,
                                                                        CancellationToken cancellationToken);
    Task<List<TrackingLogEntry.TrackingLogEntry>> DeleteTrackingLogEntryById(int userId,
                                                                             int trackingLogEntryId,
                                                                             CancellationToken cancellationToken);

    #endregion

    #region Tracking Log Entry Statuses

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntryStatus.TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(NewTrackingLogEntryStatus statusToInsert, 
                                                                                      CancellationToken cancellationToken);
    Task<List<TrackingLogEntryStatus.TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int trackingLogEntryStatusId,
                                                                                               CancellationToken cancellationToken);

    #endregion
}