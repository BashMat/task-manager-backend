#region Usings

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TaskManagerBackend.DataAccess.Database;

#endregion

namespace TaskManagerBackend.IntegrationTests;

public sealed class CustomWebApplicationFactory : WebApplicationFactory<TaskManagerBackend.Application.Program>
{
    private readonly string _connectionString;

    public CustomWebApplicationFactory(MsSqlTests fixture)
    {
        _connectionString = fixture.MsSqlContainer.GetConnectionString();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
                                  {
                                      services.Remove(services.SingleOrDefault(service => 
                                                                                   typeof(DbContextOptions<TaskManagerDbContext>) == service.ServiceType)
                                                      ?? throw new ArgumentNullException($"Could not remove {typeof(DbContextOptions<TaskManagerDbContext>)}"));
                                      services.AddDbContext<TaskManagerDbContext>((_, option) => option.UseSqlServer(_connectionString));
                                      
                                      using IServiceScope scope = services.BuildServiceProvider().CreateScope();
                                      TaskManagerDbContext dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
                                      dbContext.Database.Migrate();
                                      
                                      dbContext.Database.ExecuteSqlRaw($@"insert into [User] ([UserName], [Email], [CreatedAt], [UpdatedAt], [PasswordHash], [PasswordSalt])
                                      values ('{IntegrationTestBase.UserName}', '{IntegrationTestBase.Email}', GETUTCDATE(), GETUTCDATE(), 0x40DE696B1AD3111478314B0E7974A53890459BA391ED17BE30022888D80716947C9F6E0B8FCBFBD54979146BBFC75BE276906BE4B2E817A480050120AE1BB0AA, 0x6C69D919AC228276F287ED305C58180F7119C63E5A13E9306DA3BC63497F572D7EC6C7A55E97E6EA201851CA6E3D50A9046925CEC3E617DD7759EEE756E8709660B39FAAD0FA65682058AD8BF5E2D34009D550C3A87AD25072A7315119A23DF3C9BEE1A75AE09490EC99CA5FA71766957766689E99872FD2937BFA7BDD48DE12)");
                                  });
    }
}