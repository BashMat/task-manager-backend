namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntryStatus;

public class TrackingLogEntryStatus
{
    public int Id { get; set; }
    public int TrackingLogId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}