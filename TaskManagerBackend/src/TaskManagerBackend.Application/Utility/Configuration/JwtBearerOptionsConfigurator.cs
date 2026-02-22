#region Usings

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;
using TaskManagerBackend.Application.Utility.Security;

#endregion

namespace TaskManagerBackend.Application.Utility.Configuration;

public class JwtBearerOptionsConfigurator : IConfigureNamedOptions<JwtBearerOptions>
{
    private readonly ICryptographyService _cryptographyService;

    public JwtBearerOptionsConfigurator(ICryptographyService cryptographyService)
    {
        _cryptographyService = cryptographyService;
    }

    public void Configure(JwtBearerOptions options)
    {
        options.TokenValidationParameters = _cryptographyService.GetValidationParameters();
    }

    public void Configure(string? name, JwtBearerOptions options)
    {
        Configure(options);
    }
}