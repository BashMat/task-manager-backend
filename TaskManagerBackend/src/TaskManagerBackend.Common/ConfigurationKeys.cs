namespace TaskManagerBackend.Common;

/// <summary>
///     Provides constant values for configuration fields.
/// </summary>
public abstract class ConfigurationKeys
{
    public const string TokensSection = "Tokens";
    public const string SecretKey = "Secret";
    public const string AccessTokenLifeTimeInMinutesKey = "AccessTokenLifeTimeInMinutes";

    public const string ConnectionStringsSection = "ConnectionStrings";
    public const string ConnectionStringsDataSection = "ConnectionStringsData";
    public const string ServerKey = "Server";
    public const string DatabaseKey = "Database";
    public const string UserKey = "User";
    public const string PasswordKey = "Password";
    public const string ConnectionTimeoutKey = "ConnectionTimeout";

    public const string TaskManagerDbConnectionString = "TaskManagerDb";
}