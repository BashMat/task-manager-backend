namespace TaskManagerBackend.DataAccess.Database.Models;

public class TrackingLogEntry : IEntity
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public int TrackingLogId { get; set; }

    public int StatusId { get; set; }

    public int? Priority { get; set; }

    public decimal OrderIndex { get; set; }

    public int CreatedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public int UpdatedBy { get; set; }

    public DateTime UpdatedAt { get; set; }

    public virtual User CreatedByNavigation { get; set; } = null!;

    public virtual Status Status { get; set; } = null!;

    public virtual TrackingLog TrackingLog { get; set; } = null!;

    public virtual User UpdatedByNavigation { get; set; } = null!;

    #region IEntity Members

    public int Id { get; set; }

    #endregion
}