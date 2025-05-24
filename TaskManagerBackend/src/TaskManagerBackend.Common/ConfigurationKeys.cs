namespace TaskManagerBackend.Common;

/// <summary>
///     Provides constant values for configuration fields.
/// </summary>
public abstract class ConfigurationKeys
{
    public const string TokensSection = "Tokens";
    public const string Secret = "Secret";
    public const string AccessTokenLifeTimeInMinutes = "AccessTokenLifeTimeInMinutes";

    public const string ConnectionStrings = "ConnectionStrings";
    public const string ConnectionStringsData = "ConnectionStringsData";

    public const string TaskManagerDbConnectionString = "TaskManagerDb";
}