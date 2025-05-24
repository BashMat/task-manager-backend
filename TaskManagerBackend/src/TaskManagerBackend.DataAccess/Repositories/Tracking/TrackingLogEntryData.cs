namespace TaskManagerBackend.DataAccess.Repositories.Tracking;

public class TrackingLogEntryData
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int TrackingLogId { get; set; }
    public int StatusId { get; set; }
    public int Priority { get; set; }
    public double OrderIndex { get; set; }
    public int CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public int UpdatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
}