namespace TaskManagerBackend.DataAccess.Database;

public interface IAuditedEntity : IEntity
{
    int CreatedBy { get; }
    DateTime CreatedAt { get; }
    int UpdatedBy { get; }
    DateTime UpdatedAt { get; }
}