namespace TaskManagerBackend.DataAccess.Database.Models;

public partial class User : IEntity
{
    public int Id { get; set; }

    public string UserName { get; set; } = null!;

    public string? FirstName { get; set; }

    public string? LastName { get; set; }

    public string Email { get; set; } = null!;

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }

    public byte[] PasswordHash { get; set; } = null!;

    public byte[] PasswordSalt { get; set; } = null!;

    public virtual ICollection<TrackingLogEntryStatus> TrackingLogEntryStatusCreatedByNavigations { get; set; } = new List<TrackingLogEntryStatus>();

    public virtual ICollection<TrackingLogEntryStatus> TrackingLogEntryStatusUpdatedByNavigations { get; set; } = new List<TrackingLogEntryStatus>();

    public virtual ICollection<TrackingLog> TrackingLogCreatedByNavigations { get; set; } = new List<TrackingLog>();

    public virtual ICollection<TrackingLogEntry> TrackingLogEntryCreatedByNavigations { get; set; } = new List<TrackingLogEntry>();

    public virtual ICollection<TrackingLogEntry> TrackingLogEntryUpdatedByNavigations { get; set; } = new List<TrackingLogEntry>();

    public virtual ICollection<TrackingLog> TrackingLogUpdatedByNavigations { get; set; } = new List<TrackingLog>();
}
