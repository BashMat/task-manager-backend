#region Usings

using Testcontainers.MsSql;
using Xunit;

#endregion

namespace TaskManagerBackend.IntegrationTests;

public sealed class MsSqlTests : IAsyncLifetime
{
    public MsSqlContainer MsSqlContainer { get; } = 
        new MsSqlBuilder("mcr.microsoft.com/mssql/server:2022-CU23-ubuntu-22.04").Build();

    public async Task InitializeAsync()
    {
        await MsSqlContainer.StartAsync();
    }

    public Task DisposeAsync()
    {
        return MsSqlContainer.DisposeAsync().AsTask();
    }
}