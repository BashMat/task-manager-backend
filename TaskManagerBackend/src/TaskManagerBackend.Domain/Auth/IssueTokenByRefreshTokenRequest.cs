namespace TaskManagerBackend.Domain.Auth;

/// <summary>
///     Represents domain representation for Issuing Token request in case of using User Refresh Token.
/// </summary>
public class IssueTokenByRefreshTokenRequest
{
    public IssueTokenByRefreshTokenRequest(RefreshTokenData refreshToken)
    {
        RefreshToken = refreshToken;
    }
    
    public RefreshTokenData RefreshToken { get; }
}