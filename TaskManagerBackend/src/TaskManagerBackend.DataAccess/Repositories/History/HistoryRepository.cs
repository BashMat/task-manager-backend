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
                                            .OrderBy(e => e.EntityVersion)
                                            .Select(e => new
                                                         {
                                                             EntityTypeName = entityType.Name,
                                                             EntityId = entityId,
                                                             e.DispatchedAt,
                                                             e.User
                                                         })
                                            .ToListAsync();
        
        return events.Select(e => new HistoryEntry
                                  {
                                      DateTime = e.DispatchedAt,
                                      User = e.User.ToDomain()
                                  })
                     .ToList();
    }
}