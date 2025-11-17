using TaskManagerBackend.Domain.Entities;

namespace TaskManagerBackend.Domain.History;

public interface IHistoryRepository
{
    Task<IReadOnlyCollection<HistoryEntry>> GetEntityHistory(EntityType entityType, 
                                                             int entityId);
}