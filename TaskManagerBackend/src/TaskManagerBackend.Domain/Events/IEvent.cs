namespace TaskManagerBackend.Domain.Events;

public interface IEvent<TObject>
{
    public Guid Id { get; }
    public int EntityType { get; }
    public int EntityId { get; }
    public int EntityVersion { get; }
    public TObject Data { get; }
    public int DispatchedByUserId { get; }
    public DateTime DispatchedAt { get; }
    public Guid CorrelationId { get; }
}