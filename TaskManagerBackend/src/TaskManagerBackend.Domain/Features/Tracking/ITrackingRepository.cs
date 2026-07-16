#region Usings

using TaskManagerBackend.Domain.Features.Tracking.TrackingLog;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;
using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntryStatus;

#endregion

namespace TaskManagerBackend.Domain.Features.Tracking;

public interface ITrackingRepository
{
    #region Tracking Logs

    // TODO: Perhaps nullability is excessive
    Task<TrackingLog.TrackingLog?> CreateTrackingLog(NewTrackingLog newTrackingLog, CancellationToken cancellationToken);
    Task<List<TrackingLog.TrackingLog>> GetAllTrackingLogs(int userId, CancellationToken cancellationToken);
    Task<TrackingLog.TrackingLog?> GetTrackingLogById(int id, CancellationToken cancellationToken);
    Task<TrackingLogEntity?> GetTrackingLogEntityById(int id,
                                                      CancellationToken cancellationToken);
    Task Save(TrackingLogEntity log,
              CancellationToken cancellationToken);
    Task<List<TrackingLog.TrackingLog>> DeleteTrackingLogById(int id,
                                                              int userId,
                                                              CancellationToken cancellationToken);

    #endregion
    
    #region Tracking Log Entries

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntry.TrackingLogEntry?> CreateTrackingLogEntry(NewTrackingLogEntry newTrackingLogEntry,
                                                                    CancellationToken cancellationToken);
    Task<List<TrackingLogEntry.TrackingLogEntry>> GetAllTrackingLogEntries(int userId, 
                                                                           CancellationToken cancellationToken);
    Task<TrackingLogEntry.TrackingLogEntry?> GetTrackingLogEntryById(int id,
                                                                     CancellationToken cancellationToken);
    Task<TrackingLogEntryEntity?> GetTrackingLogEntryEntityById(int id,
                                                                CancellationToken cancellationToken);
    Task<TrackingLogEntry.TrackingLogEntry?> UpdateTrackingLogEntryById(int id,
                                                                        UpdatableTrackingLogEntry updatableTrackingLogEntry,
                                                                        CancellationToken cancellationToken);
    Task Save(TrackingLogEntryEntity logEntry,
              CancellationToken cancellationToken);
    Task<List<TrackingLogEntry.TrackingLogEntry>> DeleteTrackingLogEntryById(int id,
                                                                             int userId,
                                                                             CancellationToken cancellationToken);

    #endregion

    #region Tracking Log Entry Statuses

    // TODO: Perhaps nullability is excessive
    Task<TrackingLogEntryStatus.TrackingLogEntryStatus?> CreateTrackingLogEntryStatus(NewTrackingLogEntryStatus newStatus, 
                                                                                      CancellationToken cancellationToken);
    Task<TrackingLogEntryStatus.TrackingLogEntryStatus?> GetTrackingLogEntryStatusById(int id, 
                                                                                       CancellationToken cancellationToken);
    Task<List<TrackingLogEntryStatus.TrackingLogEntryStatus>> DeleteTrackingLogEntryStatusById(int id,
                                                                                               CancellationToken cancellationToken);

    #endregion
}