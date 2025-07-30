namespace TaskManagerBackend.DataAccess.Database.Models;

public partial class RefreshToken
{
    public int UserId { get; set; }

    public string Token { get; set; } = null!;

    public DateTime IssuedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual User User { get; set; } = null!;
}
