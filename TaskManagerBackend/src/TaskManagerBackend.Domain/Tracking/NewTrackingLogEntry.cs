namespace TaskManagerBackend.Domain.Tracking;

public class NewTrackingLogEntry
{
    public int TrackingLogId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int StatusId { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
}