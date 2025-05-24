namespace TaskManagerBackend.Domain.Tracking;

public class NewTrackingLogEntry
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int TrackingLogId { get; set; }
    public int StatusId { get; set; }
    public int? Priority { get; set; }
    public double OrderIndex { get; set; }
    public int CreatedById { get; set; }
    public DateTime CreatedAt { get; set; }
}