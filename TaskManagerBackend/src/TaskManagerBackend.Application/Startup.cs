using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Prometheus;
using Swashbuckle.AspNetCore.Filters;
using TaskManagerBackend.Application.Health;
using TaskManagerBackend.Application.Services.Auth;
using TaskManagerBackend.Application.Services.Board;
using TaskManagerBackend.Common;
using TaskManagerBackend.DataAccess.Repositories.Board;
using TaskManagerBackend.DataAccess.Repositories.User;

namespace TaskManagerBackend.Application;

public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureBuilder(WebApplicationBuilder builder)
    {
        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
                                       {
                                           options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                                                                             {
                                                                                 Description =
                                                                                     "Use the Bearer scheme: \"bearer {token}\"",
                                                                                 In = ParameterLocation.Header,
                                                                                 Name = "Authorization",
                                                                                 Type = SecuritySchemeType.ApiKey
                                                                             });
                                           options.OperationFilter<SecurityRequirementsOperationFilter>();
                                       });

        builder.Host.UseNLog();

        RegisterServices(builder.Services);

        builder.Services.AddHealthChecks()
               .AddCheck<ServiceProcessHealthCheck>(ServiceProcessHealthCheck.Name)
               .ForwardToPrometheus();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
                                                {
                                                    ValidateIssuerSigningKey = true,
                                                    IssuerSigningKey =
                                                        new SymmetricSecurityKey(Encoding.UTF8
                                                            .GetBytes(builder.Configuration[ConfigurationKeys.Token])),
                                                    ValidateIssuer = false,
                                                    ValidateAudience = false,
                                                    ValidateLifetime = true,
                                                    ClockSkew = TimeSpan.Zero
                                                };
        });

        builder.Services.AddCors(options =>
                                 {
                                     options.AddPolicy("MyDefaultPolicy",
                                                       policy =>
                                                       {
                                                           policy.AllowAnyOrigin()
                                                                 .AllowAnyHeader()
                                                                 .AllowAnyMethod();
                                                       });
                                 });
    }

    private void RegisterServices(IServiceCollection services)
    {
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IBoardService, BoardService>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<IAuthProvider, AuthProvider>();
    }

    public void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseMetricServer();

        app.MapHealthChecks(Common.HealthChecks.DefaultHealthRoute,
                            new HealthCheckOptions
                            {
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                            });

        app.UseHttpMetrics();
    }
}