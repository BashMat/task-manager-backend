using TaskManagerBackend.Domain.Users;

namespace TaskManagerBackend.Domain.History;

public class HistoryEntry
{
    public required MinimalUserData User { get; init; }
    public DateTime DateTime { get; init; }
}