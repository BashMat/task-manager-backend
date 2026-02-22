namespace TaskManagerBackend.DataAccess.Database.Models;

public partial class RefreshToken
{
    public Guid Id { get; set; }

    public int UserId { get; set; }

    public DateTime IssuedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}
