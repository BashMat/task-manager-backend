using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntry;

public class UpdateTrackingLogEntryRequest
{
    [Required]
    [MaxLength(256)]
    public required string Title { get; init; }

    [MaxLength(512)]
    public string? Description { get; init; }

    public int TrackingLogId { get; init; }
    public int StatusId { get; init; }
    public int? Priority { get; init; }
    public double OrderIndex { get; init; }
    public DateTime UpdatedAt { get; init; }
}