namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogEntryStatus
{
    public TrackingLogEntryStatus(int id,
                                  string title,
                                  string? description,
                                  int trackingLogId)
    {
        Id = id;
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
    }

    public int Id { get; init; }

    public string Title { get; init; }

    public string? Description { get; init; }

    public int TrackingLogId { get; init; }
}