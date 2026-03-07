namespace TaskManagerBackend.Domain.Auth;

public class RefreshTokenData
{
    public RefreshTokenData(int userId,
                            Guid tokenId,
                            string token,
                            DateTime issuedAt,
                            DateTime expiresAt)
    {
        UserId = userId;
        TokenId = tokenId;
        Token = token;
        IssuedAt = issuedAt;
        ExpiresAt = expiresAt;
    }
    
    public int UserId { get; }
    public Guid TokenId { get; }
    public string Token { get; }
    public DateTime IssuedAt { get; }
    public DateTime ExpiresAt { get; }
}