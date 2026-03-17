using TaskManagerBackend.Domain.Data;

namespace TaskManagerBackend.Domain.Tracking.TrackingLogEntryStatus;

public record TrackingLogEntryStatus(int Id,
                                     StringAttribute Title,
                                     StringAttribute? Description,
                                     int TrackingLogId);