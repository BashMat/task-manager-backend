using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Domain.Validation;

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;

public class TrackingLogEntryCreateRequest
{
    [Required]
    [MaxLength(Constants.MaxDefaultTextLength)]
    public required string Title { get; init; }
    
    [MaxLength(Constants.MaxLongTextLength)]
    public string? Description { get; init; }
    
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int TrackingLogId { get; init; }
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int StatusId { get; init; }
    
    [Range(Constants.MinPriority, Constants.MaxPriority)]
    public int? Priority { get; init; }
    
    [Range(Constants.MinOrderIndex, Constants.MaxOrderIndex)]
    public double OrderIndex { get; init; }
}