namespace TaskManagerBackend.Common;

/// <summary>
///     Provides constant values for health check diagnostics.
/// </summary>
public abstract class HealthChecks
{
    public const string DefaultHealthRoute = "_health";
    public const string Uptime = "Uptime";
    public const string AssemblyName = "AssemblyName";
}