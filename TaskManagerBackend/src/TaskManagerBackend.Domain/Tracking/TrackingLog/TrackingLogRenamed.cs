using TaskManagerBackend.Domain.Events;

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogRenamed : IEvent<TrackingLogRenamedData>
{
    public TrackingLogRenamed(Guid id,
                              int entityId,
                              TrackingLogRenamedData data,
                              int dispatchedByUserId,
                              DateTime dispatchedAt)
    {
        Id = id;
        EntityType = Entities.EntityType.TrackingLog.Id;
        EntityId = entityId;
        Data = data;
        DispatchedByUserId = dispatchedByUserId;
        DispatchedAt = dispatchedAt;
    }
    
    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public TrackingLogRenamedData Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId => Id;
    public Guid CausationId => Id;
}