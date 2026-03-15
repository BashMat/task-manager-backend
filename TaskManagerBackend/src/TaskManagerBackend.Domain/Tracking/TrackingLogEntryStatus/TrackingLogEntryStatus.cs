using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

public class TrackingLogEntryStatus
{
    public TrackingLogEntryStatus(int id,
                                  StringAttribute title,
                                  StringAttribute? description,
                                  int trackingLogId)
    {
        Id = id;
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
    }

    public int Id { get; init; }
    public StringAttribute Title { get; init; }
    public StringAttribute? Description { get; init; }
    public int TrackingLogId { get; init; }
}