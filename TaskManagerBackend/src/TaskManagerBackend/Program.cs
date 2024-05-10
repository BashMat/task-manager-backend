using System.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using NLog;
using NLog.Web;
using Swashbuckle.AspNetCore.Filters;
using TaskManagerBackend.Common;
using TaskManagerBackend.DataAccess.Repositories.Board;
using TaskManagerBackend.DataAccess.Repositories.User;
using TaskManagerBackend.Health;
using TaskManagerBackend.Services.Auth;
using TaskManagerBackend.Services.Board;

namespace TaskManagerBackend
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var logger = LogManager.Setup()
                                   .LoadConfigurationFromAppSettings()
                                   .GetCurrentClassLogger();
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                if (string.IsNullOrWhiteSpace(builder.Configuration[ConfigurationKeys.Token]))
                {
                    throw new ConfigurationErrorsException("Secret key for token was not specified");
                }

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

                builder.Services.AddScoped<IAuthService, AuthService>();
                builder.Services.AddScoped<IBoardService, BoardService>();
                builder.Services.AddScoped<IUserRepository, UserRepository>();
                builder.Services.AddScoped<IBoardRepository, BoardRepository>();
                builder.Services.AddScoped<IAuthProvider, AuthProvider>();

                builder.Services.AddHealthChecks()
                                .AddCheck<ServiceProcessHealthCheck>(ServiceProcessHealthCheck.Name);

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

                var app = builder.Build();

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

                app.MapHealthChecks(Common.HealthChecks.DefaultHealthRoute,
                                    new HealthCheckOptions
                                    {
                                        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
                                    });

                app.Run();
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error occured during application initialization");
                throw;
            }
            finally
            {
                LogManager.Shutdown();
            }
        }
    }
}