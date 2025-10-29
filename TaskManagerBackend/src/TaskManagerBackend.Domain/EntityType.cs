namespace TaskManagerBackend.Domain;

public class EntityType
{
    public int Id { get; }
    public string Name { get; }
    
    private EntityType(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public static EntityType User { get; } = new(1, "User");
    public static EntityType TrackingLog { get; } = new(2, "TrackingLog");
    public static EntityType TrackingLogEntryStatus { get; } = new(3, "TrackingLogEntryStatus");
    public static EntityType TrackingLogEntry { get; } = new(4, "TrackingLogEntry");
}