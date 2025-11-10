using TaskManagerBackend.Domain.History;

namespace TaskManagerBackend.Application.Features.History.Dtos;

public class GetEntityHistoryResponse
{
    public required string EntityTypeName { get; init; }
    public int EntityId { get; init; }
    public required IReadOnlyCollection<HistoryEntry> HistoryEntries { get; set; }
}