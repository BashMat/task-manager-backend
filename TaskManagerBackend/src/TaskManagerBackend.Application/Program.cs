#region Usings

using NLog;
using NLog.Web;

#endregion

namespace TaskManagerBackend.Application;

public class Program
{
    public static void Main(string[] args)
    {
        Logger logger = LogManager.Setup()
                                  .LoadConfigurationFromAppSettings()
                                  .GetCurrentClassLogger();
        try
        {
            WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

            Startup startup = new(builder.Configuration);

            startup.ConfigureBuilder(builder);

            WebApplication app = builder.Build();

            startup.ConfigureApp(app);

            app.Run();
        }
        catch (Exception ex)
        {
            logger.Error(ex, "Error occured during application initialization.");
            throw;
        }
        finally
        {
            LogManager.Shutdown();
        }
    }
}