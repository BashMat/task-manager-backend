using TaskManagerBackend.Domain.Features.Users;

namespace TaskManagerBackend.Domain.Shared.Entities;

public interface IAuditedEntityWithMinimalUserData : IEntity
{
    MinimalUserData CreatedBy { get; }
    DateTime CreatedAt { get; }
    MinimalUserData UpdatedBy { get; }
    DateTime UpdatedAt { get; }
}