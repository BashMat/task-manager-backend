namespace TaskManagerBackend.Dto.Tracking.TrackingLogEntry;

public class UpdateTrackingLogEntryRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int TrackingLogId { get; set; }
    public int StatusId { get; set; }
    public int Priority { get; set; }
    public double OrderIndex { get; set; }
    public DateTime UpdatedAt { get; set; }
}