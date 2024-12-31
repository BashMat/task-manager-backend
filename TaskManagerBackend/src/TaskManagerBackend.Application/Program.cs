#region Usings

using System.Configuration;
using NLog;
using NLog.Web;
using TaskManagerBackend.Common;

#endregion

namespace TaskManagerBackend.Application;

public class Program
{
    public static void Main(string[] args)
    {
        var logger = LogManager.Setup()
                               .LoadConfigurationFromAppSettings()
                               .GetCurrentClassLogger();
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            if (string.IsNullOrWhiteSpace(builder.Configuration[ConfigurationKeys.Token]))
            {
                throw new ConfigurationErrorsException("Secret key for token was not specified");
            }
                
            Startup startup = new(builder.Configuration);

            startup.ConfigureBuilder(builder);

            WebApplication app = builder.Build();
                
            startup.ConfigureApp(app);

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