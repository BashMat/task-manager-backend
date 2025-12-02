#region Usings

using TaskManagerBackend.Domain.Events;

#endregion

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogEntryStatusCreated : EntityCreated, IEvent<NewTrackingLogEntryStatus>
{
    public TrackingLogEntryStatusCreated(Guid id, 
                                         int entityId, 
                                         NewTrackingLogEntryStatus data)
    {
        Id = id;
        EntityType = Entities.EntityType.TrackingLogEntryStatus.Id;
        EntityId = entityId;
        Data = data;
        DispatchedByUserId = data.CreatedById;
        DispatchedAt = data.CreatedAt;
    }

    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public NewTrackingLogEntryStatus Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId => Id;
    public Guid CausationId => Id;
}