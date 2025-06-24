namespace TaskManagerBackend.DataAccess.Database.Models;

public partial class TrackingLogEntryStatus : IAuditedEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int TrackingLogId { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual TrackingLog TrackingLog { get; set; } = null!;

    public virtual ICollection<TrackingLogEntry> TrackingLogEntries { get; set; } = new List<TrackingLogEntry>();

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
