using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

public class TrackingLogEntryStatusCreateRequest
{
    public int TrackingLogId { get; init; }

    [Required]
    [MaxLength(256)]
    public required string Title { get; init; }

    [MaxLength(512)]
    public string? Description { get; init; }
}