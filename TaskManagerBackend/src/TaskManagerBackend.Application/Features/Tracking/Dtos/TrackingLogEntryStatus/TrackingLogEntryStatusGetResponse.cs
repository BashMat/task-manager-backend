namespace TaskManagerBackend.Application.Features.Tracking.Dtos.TrackingLogEntryStatus;

public class TrackingLogEntryStatusGetResponse
{
    public int Id { get; set; }
    public int TrackingLogId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}