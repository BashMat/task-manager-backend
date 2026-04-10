using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;

public class TrackingLogEntryStatusCreateRequest
{
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int TrackingLogId { get; init; }

    [Required]
    [MaxLength(Constants.MaxDefaultTextLength)]
    public string Title { get; init; } = null!;

    [MaxLength(Constants.MaxLongTextLength)]
    public string? Description { get; init; }
}