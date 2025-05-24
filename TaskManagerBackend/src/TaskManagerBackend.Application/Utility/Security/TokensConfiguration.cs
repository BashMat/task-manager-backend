namespace TaskManagerBackend.Application.Utility.Security;

public class TokensConfiguration
{
    public required string Secret { get; init; }
    public required string AccessTokenLifeTimeInMinutes { get; init; }
    public double AccessTokenLifeTimeInMinutesAsDouble => Convert.ToDouble(AccessTokenLifeTimeInMinutes);
}