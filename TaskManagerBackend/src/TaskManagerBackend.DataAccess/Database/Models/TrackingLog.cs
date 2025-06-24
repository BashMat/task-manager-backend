namespace TaskManagerBackend.DataAccess.Database.Models;

public partial class TrackingLog : IEntity
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual ICollection<Status> Statuses { get; set; } = new List<Status>();

    public virtual ICollection<TrackingLogEntry> TrackingLogEntries { get; set; } = new List<TrackingLogEntry>();

    public virtual User UpdatedByNavigation { get; set; } = null!;
}
