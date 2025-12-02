using TaskManagerBackend.Domain.Events;

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogEntryUpdated : IEvent<UpdatableTrackingLogEntry>
{
    public TrackingLogEntryUpdated(Guid id,
                                   int entityId,
                                   int entityVersion,
                                   UpdatableTrackingLogEntry data)
    {
        Id = id;
        EntityType = Entities.EntityType.TrackingLogEntry.Id;
        EntityId = entityId;
        EntityVersion = entityVersion;
        Data = data;
        DispatchedByUserId = data.UpdatedBy;
        DispatchedAt = data.UpdatedAt;
    }
    
    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public int EntityVersion { get; }
    public UpdatableTrackingLogEntry Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId => Id;
    public Guid CausationId => Id;
}