namespace TaskManagerBackend.Domain.Events;

public abstract class EntityCreated
{
    internal EntityCreated()
    {
        EntityVersion = Constants.NewEntityVersion;
    }
    
    public int EntityVersion { get; }
}