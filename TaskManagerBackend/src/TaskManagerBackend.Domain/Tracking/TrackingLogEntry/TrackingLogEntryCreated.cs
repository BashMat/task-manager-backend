#region Usings

using TaskManagerBackend.Domain.Events;

#endregion

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogEntryCreated : EntityCreated, IEvent<NewTrackingLogEntry>
{
    public TrackingLogEntryCreated(Guid id,
                                   int entityId,
                                   NewTrackingLogEntry data,
                                   Guid correlationId)
    {
        Id = id;
        EntityType = Entities.EntityType.TrackingLogEntry.Id;
        EntityId = entityId;
        Data = data;
        DispatchedByUserId = data.CreatedById;
        DispatchedAt = data.CreatedAt;
        CorrelationId = correlationId;
    }

    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public NewTrackingLogEntry Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId { get; }
}