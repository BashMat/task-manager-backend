using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Database.Models;
using TaskManagerBackend.Domain.Entities;
using TaskManagerBackend.Domain.Events;

namespace TaskManagerBackend.DataAccess;

public class EventStore : IEventStore
{
    private readonly TaskManagerDbContext _dbContext;
    
    public EventStore(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> GetLastEntityVersion(EntityType entityType,
                                                int entityId)
    {
        return await _dbContext.Events.Where(e => e.EntityType == entityType.Id &&
                                                  e.EntityId == entityId)
                                      .Select(e => e.EntityVersion)
                                      .OrderByDescending(o => o)
                                      .FirstAsync();
    }
    
    public void Append<TEntity>(IEvent<TEntity> domainEvent,
                                int entityVersion)
    {
        string dataJson = JsonSerializer.Serialize(domainEvent.Data);

        Event dbEvent = new()
                        {
                            Id = domainEvent.Id,
                            EntityId = domainEvent.EntityId,
                            EntityType = domainEvent.EntityType,
                            EntityVersion = entityVersion,
                            Data = dataJson,
                            DispatchedByUserId = domainEvent.DispatchedByUserId,
                            DispatchedAt = domainEvent.DispatchedAt,
                            CorrelationId = domainEvent.CorrelationId,
                            CausationId = domainEvent.CausationId
                        };
        
        _dbContext.Events.Add(dbEvent);
    }
}