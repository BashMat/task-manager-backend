using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

public class TrackingLogEntryStatus(int id,
                                    StringAttribute title,
                                    StringAttribute? description,
                                    int trackingLogId)
{
    public int Id { get; } = id;
    public StringAttribute Title { get; } = title;
    public StringAttribute? Description { get; } = description;
    public int TrackingLogId { get; } = trackingLogId;
}