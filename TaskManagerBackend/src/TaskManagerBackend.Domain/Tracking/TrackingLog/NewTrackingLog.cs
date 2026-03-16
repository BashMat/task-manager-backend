using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLog;

public class NewTrackingLog(StringAttribute title,
                            StringAttribute? description,
                            int createdById,
                            DateTime createdAt)
{
    public StringAttribute Title { get; } = title;
    public StringAttribute? Description { get; } = description;
    public int CreatedById { get; } = createdById;
    public DateTime CreatedAt { get; } = createdAt;
}