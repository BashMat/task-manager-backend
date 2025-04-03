#region Usings

using System.Diagnostics;
using System.Reflection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

#endregion

namespace TaskManagerBackend.Application.Health;

public class ServiceProcessHealthCheck : IHealthCheck
{
    public const string Name = "ServiceProcess";
    private const string Description = "Service process is active";

    public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context,
                                                    CancellationToken cancellationToken = new())
    {
        Dictionary<string, object> additionalData = new();
        
        Assembly? assembly = Assembly.GetEntryAssembly();
        if (!string.IsNullOrWhiteSpace(assembly?.FullName))
        {
            additionalData.Add(Common.HealthChecks.AssemblyName, assembly.FullName);
        }

        additionalData.Add(Common.HealthChecks.Uptime, DateTime.Now - Process.GetCurrentProcess().StartTime);

        return Task.FromResult(HealthCheckResult.Healthy(Description, additionalData));
    }
}