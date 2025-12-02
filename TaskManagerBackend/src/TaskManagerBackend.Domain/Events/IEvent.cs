namespace TaskManagerBackend.Domain.Events;

public interface IEvent<TEntity>
{
    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public int EntityVersion { get; }
    public TEntity Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId { get; }
    public Guid CausationId { get; }
}