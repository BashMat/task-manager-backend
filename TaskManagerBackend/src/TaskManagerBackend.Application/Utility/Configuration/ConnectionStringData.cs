namespace TaskManagerBackend.Application.Utility.Configuration;

/// <summary>
///     Represents data object holding values for Connection String
/// </summary>
public class ConnectionStringData
{
    public string? Server { get; init; }
    public string? Database { get; init; }
    public int? ConnectionTimeout { get; init; }
    public string? User { get; init; }
    public string? Password { get; init; }
}