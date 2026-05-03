namespace TaskManagerBackend.Application.Utility.Security;

public class TokensConfiguration
{
    public required string Secret { get; init; }
    public int AccessTokenLifeTimeInMinutes { get; init; }
    public int RefreshTokenLifeTimeInMinutes { get; init; }
}