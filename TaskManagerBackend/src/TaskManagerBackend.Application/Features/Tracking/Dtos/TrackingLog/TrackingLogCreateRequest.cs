using System.ComponentModel.DataAnnotations;

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLog;

public class TrackingLogCreateRequest
{
    [Required]
    [MaxLength(256)]
    public required string Title { get; set; }
    
    [MaxLength(512)]
    public string? Description { get; set; }
}