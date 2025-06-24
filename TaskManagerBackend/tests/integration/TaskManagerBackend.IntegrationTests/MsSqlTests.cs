#region Usings

using Dapper;
using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests;

public sealed class MsSqlTests : IAsyncLifetime
{
    public MsSqlContainer MsSqlContainer { get; } = new MsSqlBuilder().Build();

    public async Task InitializeAsync()
    {
        await MsSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return MsSqlContainer.DisposeAsync().AsTask();
    }
}