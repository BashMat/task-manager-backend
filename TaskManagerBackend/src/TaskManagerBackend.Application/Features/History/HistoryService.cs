using TaskManagerBackend.Application.Features.History.Dtos;
using TaskManagerBackend.Domain.Entities;
using TaskManagerBackend.Domain.History;

namespace TaskManagerBackend.Application.Features.History;

public class HistoryService : IHistoryService
{
    private readonly IHistoryRepository _historyRepository;

    public HistoryService(IHistoryRepository historyRepository)
    {
        _historyRepository = historyRepository;
    }
    
    // TODO: Add check of user permissions by entity type and id.
    public async Task<GetEntityHistoryResponse> GetEntityHistory(EntityType entityType, 
                                                                 int entityId)
    {
        IReadOnlyCollection<HistoryEntry> historyEntries = 
            await _historyRepository.GetEntityHistory(entityType, 
                                                      entityId);
        return new GetEntityHistoryResponse
               {
                   EntityTypeName = entityType.Name,
                   EntityId = entityId,
                   HistoryEntries = historyEntries
               };
    }
}