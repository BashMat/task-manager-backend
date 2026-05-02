using TaskManagerBackend.Domain.Shared.Data;

namespace TaskManagerBackend.Domain.Features.Tracking.TrackingLogEntryStatus;

public record TrackingLogEntryStatus(int Id,
                                     StringAttribute Title,
                                     StringAttribute? Description,
                                     int TrackingLogId);