using TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntry;

namespace TaskManagerBackend.Domain.Features.Tracking;

public interface ITrackingQueries
{
    Task<TrackingLogEntryEntity?> GetTopOrderedTrackingLogEntryByStatusId(int statusId,
                                                                          CancellationToken cancellationToken);
}