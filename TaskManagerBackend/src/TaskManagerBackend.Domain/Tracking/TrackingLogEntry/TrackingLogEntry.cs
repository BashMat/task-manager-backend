#region

using TaskManagerBackend.Domain.Data;
using TaskManagerBackend.Domain.Entities;
using TaskManagerBackend.Domain.Users;

#endregion

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntry;

public class TrackingLogEntry(int id,
                              StringAttribute title,
                              StringAttribute? description,
                              int trackingLogId,
                              TrackingLogEntryStatus.TrackingLogEntryStatus status,
                              int? priority,
                              decimal orderIndex,
                              MinimalUserData createdBy,
                              DateTime createdAt,
                              MinimalUserData updatedBy,
                              DateTime updatedAt) : IAuditedEntityWithMinimalUserData
{
    public int Id { get; } = id;
    public StringAttribute Title { get; } = title;
    public StringAttribute? Description { get; } = description;

    public int TrackingLogId { get; } = trackingLogId;

    public TrackingLogEntryStatus.TrackingLogEntryStatus Status { get; } = status;

    public int? Priority { get; } = priority;

    public decimal OrderIndex { get; } = orderIndex;

    public MinimalUserData CreatedBy { get; } = createdBy;

    public DateTime CreatedAt { get; } = createdAt;

    public MinimalUserData UpdatedBy { get; } = updatedBy;

    public DateTime UpdatedAt { get; } = updatedAt;
}