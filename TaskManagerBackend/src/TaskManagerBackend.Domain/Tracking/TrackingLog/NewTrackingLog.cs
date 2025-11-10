using TaskManagerBackend.Common.Services;

namespace TaskManagerBackend.Domain.Tracking;

public class NewTrackingLog
{
    public NewTrackingLog(string title,
                          string? description,
                          int createdById,
                          IDateTimeService dateTimeService)
    {
        Title = title;
        Description = description;
        CreatedById = createdById;
        CreatedAt = dateTimeService.UtcNow;
    }

    public string Title { get; }
    public string? Description { get; }
    public int CreatedById { get; }
    public DateTime CreatedAt { get; }
}