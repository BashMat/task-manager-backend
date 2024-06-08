namespace TaskManagerBackend.Application.Configuration;

public class ConnectionStringData
{
    public string? Server { get; init; }
    public string? Database { get; init; }
    public int? ConnectionTimeout { get; init; }
    public string? User { get; init; }
    public string? Password { get; init; }
}