namespace TaskManagerBackend.DataAccess.Database.Models;

public partial class User
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

    public virtual ICollection<Status> StatusCreatedByNavigations { get; set; } = new List<Status>();

    public virtual ICollection<Status> StatusUpdatedByNavigations { get; set; } = new List<Status>();

    public virtual ICollection<TrackingLog> TrackingLogCreatedByNavigations { get; set; } = new List<TrackingLog>();

    public virtual ICollection<TrackingLogEntry> TrackingLogEntryCreatedByNavigations { get; set; } = new List<TrackingLogEntry>();

    public virtual ICollection<TrackingLogEntry> TrackingLogEntryUpdatedByNavigations { get; set; } = new List<TrackingLogEntry>();

    public virtual ICollection<TrackingLog> TrackingLogUpdatedByNavigations { get; set; } = new List<TrackingLog>();
}
