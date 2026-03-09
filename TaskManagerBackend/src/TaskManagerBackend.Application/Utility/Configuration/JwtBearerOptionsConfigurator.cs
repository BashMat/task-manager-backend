#region Usings

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using TaskManagerBackend.Application.Utility.Security;

#endregion

namespace TaskManagerBackend.Application.Utility.Configuration;

public class JwtBearerOptionsConfigurator(ICryptographyService cryptographyService)
    : IConfigureNamedOptions<JwtBearerOptions>
{
    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = cryptographyService.GetValidationParameters();
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}