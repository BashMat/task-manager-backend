namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;

public class TrackingLogEntryStatusGetResponse
{
    public int Id { get; init; }
    public int TrackingLogId { get; init; }
    public required string Title { get; init; }
    public string? Description { get; init; }
}