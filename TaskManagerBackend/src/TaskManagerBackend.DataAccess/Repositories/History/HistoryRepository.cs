using Microsoft.EntityFrameworkCore;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Repositories.Tracking;
using TaskManagerBackend.Domain.Entities;
using TaskManagerBackend.Domain.History;

namespace TaskManagerBackend.DataAccess.Repositories.History;

public class HistoryRepository : IHistoryRepository
{
    private readonly TaskManagerDbContext _dbContext;
    
    public HistoryRepository(TaskManagerDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public async Task<IReadOnlyCollection<HistoryEntry>> GetEntityHistory(EntityType entityType, 
                                                                          int entityId)
    {
        var events = await _dbContext.Events.AsNoTracking()
                                            .Where(e => e.EntityId == entityId && e.EntityType == entityType.Id)
                                            .Select(e => new
                                                         {
                                                             EntityTypeName = entityType.Name,
                                                             EntityId = entityId,
                                                             e.EntityVersion,
                                                             e.Data,
                                                             e.DispatchedAt,
                                                             e.User
                                                         })
                                            .OrderBy(e => e.EntityVersion)
                                            .ToListAsync();
        
        return events.Select(e => new HistoryEntry
                                  {
                                      DateTime = e.DispatchedAt,
                                      User = e.User.ToDomain(),
                                      Entity = e.Data
                                  })
                     .ToList();
    }
}