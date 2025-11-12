namespace TaskManagerBackend.Domain.Events;

public abstract class EntityCreated
{
    private const int NewEntityVersion = 1;
    public int EntityVersion => NewEntityVersion;
}