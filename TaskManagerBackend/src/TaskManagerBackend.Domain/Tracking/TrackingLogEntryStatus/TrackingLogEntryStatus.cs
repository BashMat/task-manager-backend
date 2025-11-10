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

    public int Id { get; }
    public string Title { get; }
    public string? Description { get; }
    public int TrackingLogId { get; }
}