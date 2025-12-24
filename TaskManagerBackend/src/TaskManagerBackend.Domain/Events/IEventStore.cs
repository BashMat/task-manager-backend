using TaskManagerBackend.Domain.Entities;

namespace TaskManagerBackend.Domain.Events;

public interface IEventStore
{
    Task<int> GetLastEntityVersion(EntityType entityType,
                                   int entityId);
    void Append<TEntity>(IEvent<TEntity> domainEvent,
                         int entityVersion);
}