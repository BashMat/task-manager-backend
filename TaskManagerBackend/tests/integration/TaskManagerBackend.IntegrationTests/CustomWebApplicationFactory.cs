#region Usings

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TaskManagerBackend.DataAccess;

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
                                      services.Remove(services.SingleOrDefault(service => typeof(SqlServerDbConnectionProvider) == service.ServiceType));
                                      services.AddScoped<IDbConnectionProvider<SqlConnection>>(_ => dbConnectionProviderMock.Object);
                                  });
    }
}