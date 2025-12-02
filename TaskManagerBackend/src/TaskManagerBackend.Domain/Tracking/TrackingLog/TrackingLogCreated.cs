using TaskManagerBackend.Domain.Events;

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogCreated : EntityCreated, IEvent<NewTrackingLog>
{
    public TrackingLogCreated(Guid id,
                              int entityId,
                              NewTrackingLog data)
    {
        Id = id;
        EntityType = Entities.EntityType.TrackingLog.Id;
        EntityId = entityId;
        Data = data;
        DispatchedByUserId = data.CreatedById;
        DispatchedAt = data.CreatedAt;
    }
    
    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public NewTrackingLog Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId => Id;
    public Guid CausationId => Id;
}