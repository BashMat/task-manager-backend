using TaskManagerBackend.Common.Services;

namespace TaskManagerBackend.Domain.Tracking;

public class UpdatableTrackingLogEntry
{
    public required string Title { get; init; }
    public string? Description { get; init; }
    public int TrackingLogId { get; init; }
    public int StatusId { get; init; }
    public int? Priority { get; init; }
    public double OrderIndex { get; init; }
    public int UpdatedBy { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public void SetUpdatedData(int userId, IDateTimeService dateTimeService)
    {
        UpdatedBy = userId;
        UpdatedAt = dateTimeService.UtcNow;
    }
}