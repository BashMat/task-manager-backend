using System.Configuration;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Prometheus;
using Swashbuckle.AspNetCore.Filters;
using TaskManagerBackend.Application.Configuration;
using TaskManagerBackend.Application.Health;
using TaskManagerBackend.Application.Services.Auth;
using TaskManagerBackend.Application.Services.Board;
using TaskManagerBackend.Application.Services.Email;
using TaskManagerBackend.Common;
using TaskManagerBackend.Common.Services;
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
        BuildConnectionStrings();
        
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
                                                            .GetBytes(_configuration[ConfigurationKeys.Token]!)),
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

    private void BuildConnectionStrings()
    {
        IConfigurationSection connectionStringsDataSection = _configuration.GetSection(ConfigurationKeys.ConnectionStringsData);
        IConfigurationSection connectionStringsSection = _configuration.GetSection(ConfigurationKeys.ConnectionStrings);

        if (!connectionStringsSection.Exists() || !connectionStringsDataSection.Exists())
        {
            return;
        }

        Dictionary<string, ConnectionStringData> connectionStringsData =
            ParseConnectionStringsDataSection(connectionStringsDataSection);

        foreach (IConfigurationSection connectionString in connectionStringsSection.GetChildren())
        {
            string? connectionStringValue = connectionString.Value;
            SqlConnectionStringBuilder connectionStringBuilder = new(connectionStringValue);
            ConnectionStringData? data = connectionStringsData.FirstOrDefault(d => d.Key == connectionString.Key).Value;

            if (data is not null)
            {
                connectionStringBuilder.DataSource = data.Server ?? connectionStringBuilder.DataSource;
                connectionStringBuilder.InitialCatalog = data.Database ?? connectionStringBuilder.InitialCatalog;
                connectionStringBuilder.UserID = data.User ?? connectionStringBuilder.UserID;
                connectionStringBuilder.Password = data.Password ?? connectionStringBuilder.Password;
                connectionStringBuilder.ConnectTimeout = data.ConnectionTimeout ?? connectionStringBuilder.ConnectTimeout;
            }

            _configuration[connectionString.Path] = connectionStringBuilder.ConnectionString;
        }
    }

    private Dictionary<string, ConnectionStringData> ParseConnectionStringsDataSection(IConfigurationSection connectionStringsDataSection)
    {
        Dictionary<string, ConnectionStringData> result = new();
        
        IEnumerable<IConfigurationSection> connectionStringsData = connectionStringsDataSection.GetChildren();
        
        foreach (var connectionStringData in connectionStringsData)
        {
            if (!connectionStringData.Exists())
            {
                throw new ConfigurationErrorsException("ConnectionStringData must not include empty subsections");
            }
            
            if (result.ContainsKey(connectionStringData.Key))
            {
                throw new ConfigurationErrorsException("Configuration must specify each connection string only once");
            }

            ConnectionStringData data = new()
                                        {
                                            Server = connectionStringData.GetSection("Server").Value,
                                            Database = connectionStringData.GetSection("Database").Value,
                                            User = connectionStringData.GetSection("User").Value,
                                            Password = connectionStringData.GetSection("Password").Value,
                                            ConnectionTimeout = connectionStringData.GetSection("ConnectionTimeout").Value == null 
                                                                    ? null
                                                                    : int.Parse(connectionStringData.GetSection("ConnectionTimeout").Value!)
                                        };

            result.Add(connectionStringData.Key, data);
        }

        return result;
    }

    private void RegisterServices(IServiceCollection services)
    {
        // First adding base services for possible common functionality.
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<IEmailService, EmailService>();
        
        // Then register services for main functionality
        // Auth and users:
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthProvider, AuthProvider>();
        services.AddScoped<IAuthService, AuthService>();
        
        // Boards:
        services.AddScoped<IBoardRepository, BoardRepository>();
        services.AddScoped<IBoardService, BoardService>();
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