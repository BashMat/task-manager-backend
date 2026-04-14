using TaskManagerBackend.Domain.Users;

namespace TaskManagerBackend.Domain.Entities;

public interface IAuditedEntityWithMinimalUserData : IEntity
{
    MinimalUserData CreatedBy { get; }
    DateTime CreatedAt { get; }
    MinimalUserData UpdatedBy { get; }
    DateTime UpdatedAt { get; }
}