namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

public class TrackingLogEntryStatusCreateRequest
{
    public int TrackingLogId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}