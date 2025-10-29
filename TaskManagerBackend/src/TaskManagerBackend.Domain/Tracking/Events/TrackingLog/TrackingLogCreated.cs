using TaskManagerBackend.Domain.Events;

namespace TaskManagerBackend.Domain.Tracking.Events.TrackingLog;

public class TrackingLogCreated : EntityCreated, IEvent<NewTrackingLog>
{
    public TrackingLogCreated(Guid id,
                              int entityId,
                              NewTrackingLog data,
                              int dispatchedByUserId,
                              DateTime dispatchedAt,
                              Guid correlationId)
    {
        Id = id;
        EntityType = Domain.EntityType.TrackingLog.Id;
        EntityId = entityId;
        Data = data;
        DispatchedByUserId = dispatchedByUserId;
        DispatchedAt = dispatchedAt;
        CorrelationId = correlationId;
    }
    
    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public NewTrackingLog Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId { get; }
}