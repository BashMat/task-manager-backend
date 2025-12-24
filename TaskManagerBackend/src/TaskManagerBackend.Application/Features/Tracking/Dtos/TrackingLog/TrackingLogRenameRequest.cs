using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;

public class TrackingLogRenameRequest
{
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int Id { get; init; }
    
    [Required]
    [MaxLength(Constants.MaxDefaultTextLength)]
    public required string Title { get; init; }
}