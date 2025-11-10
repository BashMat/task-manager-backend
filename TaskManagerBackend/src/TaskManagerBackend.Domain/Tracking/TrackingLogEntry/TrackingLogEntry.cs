#region

using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Domain.Tracking;

public class TrackingLogEntry
{
    public TrackingLogEntry(int id,
                            string title,
                            string? description,
                            int trackingLogId,
                            TrackingLogEntryStatus status,
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

    public int Id { get; }
    public string Title { get; }
    public string? Description { get; }
    public int TrackingLogId { get; }
    public TrackingLogEntryStatus Status { get; }
    public int? Priority { get; }
    public decimal OrderIndex { get; }
    public MinimalUserData CreatedBy { get; }
    public DateTime CreatedAt { get; }
    public MinimalUserData UpdatedBy { get; }
    public DateTime UpdatedAt { get; }
}