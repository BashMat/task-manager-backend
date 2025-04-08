namespace TaskManagerBackend.Dto.Tracking.TrackingLog;

public class NewTrackingLog
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
}