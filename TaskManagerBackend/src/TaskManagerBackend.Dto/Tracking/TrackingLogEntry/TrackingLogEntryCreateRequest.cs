namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntry;

public class TrackingLogEntryCreateRequest
{
    public int TrackingLogId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int StatusId { get; set; }
}