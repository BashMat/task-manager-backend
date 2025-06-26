using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;

public class TrackingLogEntryStatusCreateRequest
{
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int TrackingLogId { get; init; }

    [Required]
    [MaxLength(Constants.MaxDefaultTextLength)]
    public required string Title { get; init; }

    [MaxLength(Constants.MaxLongTextLength)]
    public string? Description { get; init; }
}