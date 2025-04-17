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

        await MigrateDatabase();
    }

    private async Task MigrateDatabase()
    {
        await using SqlConnection connection = new(MsSqlContainer.GetConnectionString());

        string sql = @"
create table [User]
(
	[Id] int primary key identity(1, 1),
	[UserName] nvarchar(256) not null,
	[FirstName] nvarchar(256),
	[LastName] nvarchar(256),
	[Email] nvarchar(256) not null,
	[CreatedAt] datetime2 not null,
	[UpdatedAt] datetime2 not null,
	[PasswordHash] varbinary(256) not null,
	[PasswordSalt] varbinary(256) not null
)";

        await connection.ExecuteAsync(sql);
    }

    public Task DisposeAsync()
    {
        return MsSqlContainer.DisposeAsync().AsTask();
    }
}