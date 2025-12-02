using System.Text.Json;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Domain.Events;

namespace TaskManagerBackend.DataAccess;

public class EventStore : IEventStore
{
    private readonly TaskManagerDbContext _dbContext;
    
    public EventStore(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Append<TEntity>(IEvent<TEntity> domainEvent)
    {
        string dataJson = JsonSerializer.Serialize(domainEvent.Data);

        Event dbEvent = new()
                        {
                            Id = domainEvent.Id,
                            EntityId = domainEvent.EntityId,
                            EntityType = domainEvent.EntityType,
                            EntityVersion = domainEvent.EntityVersion,
                            Data = dataJson,
                            DispatchedByUserId = domainEvent.DispatchedByUserId,
                            DispatchedAt = domainEvent.DispatchedAt,
                            CorrelationId = domainEvent.CorrelationId,
                            CausationId = domainEvent.CausationId
                        };
        
        _dbContext.Events.Add(dbEvent);
    }
}