using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;

public class TrackingLogCreateRequest
{
    [Required]
    [MaxLength(Constants.MaxDefaultTextLength)]
    public string Title { get; init; } = null!;
    
    [MaxLength(Constants.MaxLongTextLength)]
    public string? Description { get; init; }
}