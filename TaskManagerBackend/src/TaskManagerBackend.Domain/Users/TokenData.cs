namespace TaskManagerBackend.Domain.Users;

public class TokenData
{
    public int UserId { get; init; }
    public required string Token { get; init; }
    public DateTime IssuedAt { get; init; }
    public DateTime ExpiresAt { get; init; }
}