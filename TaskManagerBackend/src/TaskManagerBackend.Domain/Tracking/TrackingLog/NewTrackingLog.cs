using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLog;

public class NewTrackingLog
{
    public NewTrackingLog(StringAttribute title,
                          StringAttribute? description,
                          int createdById,
                          DateTime createdAt)
    {
        Title = title;
        Description = description;
        CreatedById = createdById;
        CreatedAt = createdAt;
    }
    
    public StringAttribute Title { get; }
    public StringAttribute? Description { get; }
    public int CreatedById { get; }
    public DateTime CreatedAt { get; }
}