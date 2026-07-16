using System.ComponentModel.DataAnnotations;
using TaskManagerBackend.Application.Utility.Json;
using TaskManagerBackend.Domain.Shared.Validation;

namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntry;

public class TrackingLogEntryEditRequest
{
    [Range(Constants.MinIdentifier, int.MaxValue)]
    public int Id { get; init; }
    
    public Optional<string> Title { get; init; }
    public Optional<int> TrackingLogEntryStatusId { get; init; }
    public Optional<string> Description { get; init; }
}