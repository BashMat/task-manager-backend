using TaskManagerBackend.Application.Features.History.Dtos;
using TaskManagerBackend.Domain.Entities;

namespace TaskManagerBackend.Application.Features.History;

public interface IHistoryService
{
    Task<GetEntityHistoryResponse> GetEntityHistory(EntityType entityType,
                                                    int entityId);
}