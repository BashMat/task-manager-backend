﻿#region Usings

using System.Configuration;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using NLog.Web;
using Prometheus;
using TaskManagerBackend.Application.Exceptions;
using TaskManagerBackend.Application.Features.Auth;
using TaskManagerBackend.Application.Features.Tracking;
using TaskManagerBackend.Application.Utility;
using TaskManagerBackend.Application.Utility.Configuration;
using TaskManagerBackend.Application.Utility.Health;
using TaskManagerBackend.Application.Utility.Security;
using TaskManagerBackend.Common.Services;
using TaskManagerBackend.DataAccess.Database;
using TaskManagerBackend.DataAccess.Repositories.Tracking;
using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Domain.Tracking;
using TaskManagerBackend.Domain.Users;
using TaskManagerBackend.Domain.Validation;

#endregion

namespace TaskManagerBackend.Application;

/// <summary>
/// Represents helping class to configure application.
/// </summary>
public class Startup
{
    private readonly IConfiguration _configuration;

    public Startup(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public void ConfigureBuilder(WebApplicationBuilder builder)
    {
        SetUpConfiguration(builder);
        
        builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
        
        // Add services to the container.
        builder.Services.AddControllers();

        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(options =>
                                       {
                                           options.SwaggerDoc("v1", new OpenApiInfo
                                                                    {
                                                                        Version = "v1",
                                                                        Title = "Task Manager Backend",
                                                                        Description = "An ASP.NET Core Web API for managing tasks",
                                                                    });
                                           options.AddSecurityDefinition("bearerAuth", new OpenApiSecurityScheme
                                                                             {
                                                                                 Type = SecuritySchemeType.Http,
                                                                                 Scheme = JwtBearerDefaults.AuthenticationScheme,
                                                                                 BearerFormat = "JWT",
                                                                                 Description = "JWT Authorization header using the Bearer scheme."
                                                                             });
                                           options.OperationFilter<SwaggerAuthorizeCheckOperationFilter>();
                                       });

        builder.Host.UseNLog();

        builder.Services.AddDbContext<TaskManagerDbContext>(options =>
                                                                options.UseSqlServer(_configuration
                                                                                     .GetConnectionString(ConfigurationKeys
                                                                                                              .TaskManagerDbConnectionString)));
        
        RegisterServices(builder.Services);

        builder.Services.AddHealthChecks()
                        .AddCheck<ServiceProcessHealthCheck>(ServiceProcessHealthCheck.Name)
                        .ForwardToPrometheus();
        
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

        builder.Services.AddCors(options =>
                                 {
                                     options.AddDefaultPolicy(policy =>
                                                              {
                                                                  policy.AllowAnyOrigin()
                                                                        .AllowAnyHeader()
                                                                        .AllowAnyMethod();
                                                              });
                                 });
    }

    private void SetUpConfiguration(WebApplicationBuilder builder)
    {
        BuildConnectionStrings();
        ValidateConfiguration();
        builder.Services.Configure<TokensConfiguration>(_configuration.GetRequiredSection(ConfigurationKeys.TokensSection));
        builder.Services.ConfigureOptions<JwtBearerOptionsConfigurator>();
    }

    private void ValidateConfiguration()
    {
        IConfigurationSection configurationSection = _configuration.GetRequiredSection(ConfigurationKeys.TokensSection);
        
        if (string.IsNullOrWhiteSpace(configurationSection[ConfigurationKeys.SecretKey]))
        {
            throw new ConfigurationErrorsException("Secret key for token was not specified.");
        }

        string? accessTokenLifeTime = configurationSection[ConfigurationKeys.AccessTokenLifeTimeInMinutesKey];
        if (string.IsNullOrWhiteSpace(accessTokenLifeTime)
            || !Double.TryParse(accessTokenLifeTime, out _))
        {
            throw new ConfigurationErrorsException("Access token lifetime duration is invalid.");
        }
    }

    private void BuildConnectionStrings()
    {
        IConfigurationSection connectionStringsDataSection = _configuration.GetSection(ConfigurationKeys.ConnectionStringsDataSection);
        IConfigurationSection connectionStringsSection = _configuration.GetSection(ConfigurationKeys.ConnectionStringsSection);

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
        
        foreach (IConfigurationSection connectionStringData in connectionStringsData)
        {
            if (!connectionStringData.Exists())
            {
                throw new ConfigurationErrorsException("ConnectionStringData must not include empty subsections.");
            }
            
            if (result.ContainsKey(connectionStringData.Key))
            {
                throw new ConfigurationErrorsException("Configuration must specify each connection string only once.");
            }

            ConnectionStringData data = new()
                                        {
                                            Server = connectionStringData[ConfigurationKeys.ServerKey],
                                            Database = connectionStringData[ConfigurationKeys.DatabaseKey],
                                            User = connectionStringData[ConfigurationKeys.UserKey],
                                            Password = connectionStringData[ConfigurationKeys.PasswordKey],
                                            ConnectionTimeout = connectionStringData[ConfigurationKeys.ConnectionTimeoutKey] == null 
                                                                    ? null
                                                                    : int.Parse(connectionStringData[ConfigurationKeys.ConnectionTimeoutKey]!)
                                        };

            result.Add(connectionStringData.Key, data);
        }

        return result;
    }

    private void RegisterServices(IServiceCollection services)
    {
        // Common and Application
        services.AddSingleton<ICryptographyService, CryptographyService>();
        services.AddSingleton<IDateTimeService, DateTimeService>();
        
        // Domain
        services.AddScoped<IEmailValidator, EmailValidator>();
        
        // Vertical feature slice
        // Auth and users:
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IAuthService, AuthService>();
        
        // Tracking:
        services.AddScoped<ITrackingRepository, TrackingRepository>();
        services.AddScoped<ITrackingService, TrackingService>();
    }

    public void ConfigureApp(WebApplication app)
    {
        // TODO: Remove in .NET 9+.
        // Hack around possible bug:
        // see PR for ASP.NET Core repo PR at https://github.com/dotnet/aspnetcore/pull/56616
        app.UseExceptionHandler(_ => { });
        
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHsts();
        }

        //app.UseHttpsRedirection();

        app.UseCors();

        app.UseAuthentication();

        app.UseAuthorization();

        app.MapControllers();

        app.UseMetricServer();

        app.MapHealthChecks(Utility.Health.HealthChecks.DefaultHealthRoute,
                            new HealthCheckOptions
                            {
                                ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                            });

        app.UseHttpMetrics();
    }
}