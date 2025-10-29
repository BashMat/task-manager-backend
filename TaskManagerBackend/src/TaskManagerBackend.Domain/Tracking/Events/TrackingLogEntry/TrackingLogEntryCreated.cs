#region Usings

using TaskManagerBackend.Domain.Events;

#endregion

namespace TaskManagerBackend.Domain.Tracking.Events.TrackingLogEntry;

public class TrackingLogEntryCreated : EntityCreated, IEvent<NewTrackingLogEntry>
{
    public TrackingLogEntryCreated(Guid id,
                                   int entityId,
                                   NewTrackingLogEntry data,
                                   int dispatchedByUserId,
                                   DateTime dispatchedAt,
                                   Guid correlationId)
    {
        Id = id;
        EntityType = Domain.EntityType.TrackingLogEntry.Id;
        EntityId = entityId;
        Data = data;
        DispatchedByUserId = dispatchedByUserId;
        DispatchedAt = dispatchedAt;
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