namespace TaskManagerBackend.Domain.Events;

public interface IEventStore
{
    void Append<TEntity>(IEvent<TEntity> domainEvent);
}