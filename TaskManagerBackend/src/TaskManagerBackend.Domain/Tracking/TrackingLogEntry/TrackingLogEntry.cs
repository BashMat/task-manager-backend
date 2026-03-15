#region

using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntry;

public class TrackingLogEntry
{
    public TrackingLogEntry(int id,
                            StringAttribute title,
                            StringAttribute? description,
                            int trackingLogId,
                            TrackingLogEntryStatus.TrackingLogEntryStatus status,
                            int? priority,
                            decimal orderIndex,
                            MinimalUserData createdBy,
                            DateTime createdAt,
                            MinimalUserData updatedBy,
                            DateTime updatedAt)
    {
        Id = id;
        Title = title;
        Description = description;
        TrackingLogId = trackingLogId;
        Status = status;
        Priority = priority;
        OrderIndex = orderIndex;
        CreatedBy = createdBy;
        CreatedAt = createdAt;
        UpdatedBy = updatedBy;
        UpdatedAt = updatedAt;
    }

    public int Id { get; init; }
    public StringAttribute Title { get; init; }
    public StringAttribute? Description { get; init; }

    public int TrackingLogId { get; init; }

    public TrackingLogEntryStatus.TrackingLogEntryStatus Status { get; init; }

    public int? Priority { get; init; }

    public decimal OrderIndex { get; init; }

    public MinimalUserData CreatedBy { get; init; }

    public DateTime CreatedAt { get; init; }

    public MinimalUserData UpdatedBy { get; init; }

    public DateTime UpdatedAt { get; init; }
}