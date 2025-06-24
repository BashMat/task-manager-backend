#region Usings

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaskManagerBackend.DataAccess;
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
        Mock<IDbConnectionProvider<SqlConnection>> dbConnectionProviderMock = new();
        dbConnectionProviderMock.Setup(o => o.GetConnection())
                                .Returns(() => new SqlConnection(_connectionString));
        
        builder.ConfigureServices(services =>
                                  {
                                      services.Remove(services.SingleOrDefault(service => 
                                                                                   typeof(SqlServerDbConnectionProvider) == service.ImplementationType)
                                                      ?? throw new ArgumentNullException($"Could not remove {typeof(SqlServerDbConnectionProvider)}"));
                                      services.AddScoped<IDbConnectionProvider<SqlConnection>>(_ => dbConnectionProviderMock.Object);
                                      services.Remove(services.SingleOrDefault(service => 
                                                                                   typeof(DbContextOptions<TaskManagerDbContext>) == service.ServiceType)
                                                      ?? throw new ArgumentNullException($"Could not remove {typeof(DbContextOptions<TaskManagerDbContext>)}"));
                                      services.AddDbContext<TaskManagerDbContext>((_, option) => option.UseSqlServer(_connectionString));
                                      
                                      using IServiceScope scope = services.BuildServiceProvider().CreateScope();
                                      TaskManagerDbContext dbContext = scope.ServiceProvider.GetRequiredService<TaskManagerDbContext>();
                                      dbContext.Database.Migrate();
                                      
                                      dbContext.Database.ExecuteSqlRaw($@"insert into [User] ([UserName], [Email], [CreatedAt], [UpdatedAt], [PasswordHash], [PasswordSalt])
                                      values ('{IntegrationTestBase.UserName}', '{IntegrationTestBase.Email}', GETUTCDATE(), GETUTCDATE(), 0x0DDDB41591C33FD4CF3ADE187DC85475BC00F1BD29BD15D88456F6BF48C97322938D3C8F9F31476B9F5E1EA983680F08339C8EB9814D37930A17A3A6C4521329, 0x3248D43ABB0866DE81683FC7CDDE1EB2F01852515E208796B2220006CFC5997ABD1A4AA05BE6F35569E73562EB08F8DEE136733BACD41DDBC7E8F39F4A036298962CFEBD72293CF1CBD97EA9F4C107045D112224ECDF8001D276F13B79C527B72ADBC2742D50DAF07C0E02F93D15AD275C59275E173D05D730854C3064563E52)");
                                  });
    }
}