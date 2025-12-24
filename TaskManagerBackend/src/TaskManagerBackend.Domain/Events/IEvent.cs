namespace TaskManagerBackend.Domain.Events;

public interface IEvent
{
    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId { get; }
    public Guid CausationId { get; }
}

public interface IEvent<TEntity> : IEvent
{
    public TEntity Data { get; }
}