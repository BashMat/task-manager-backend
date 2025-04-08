namespace TaskManagerBackend.Dto.Tracking.TrackingLog;

public class TrackingLogCreateRequest
{
    public required string Title { get; set; }
    public string? Description { get; set; }
}