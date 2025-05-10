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

        string sql = @$"
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
)

create table [TrackingLog]
(
    [Id] int primary key identity(1, 1),
    [Title] nvarchar(256) not null,
    [Description] nvarchar(512),
    [CreatedBy] int not null,
    [CreatedAt] datetime2 not null,
    [UpdatedBy] int not null,
    [UpdatedAt] datetime2 not null,

    constraint [TrackingLog_CreatedBy_FK] 
    foreign key ([CreatedBy]) references [User] ([Id]),
    constraint [TrackingLog_UpdatedBy_FK]
    foreign key ([UpdatedBy]) references [User] ([Id]),
)

create table [Status]
(
    [Id] int primary key identity(1, 1),
    [Title] nvarchar(256) not null,
    [Description] nvarchar(512),
    [TrackingLogId] int not null,
    [CreatedBy] int not null,
    [CreatedAt] datetime2 not null,
    [UpdatedBy] int not null,
    [UpdatedAt] datetime2 not null,

    constraint [Status_TrackingLogId_FK]
    foreign key ([TrackingLogId]) references [TrackingLog] ([Id]),
    constraint [Status_CreatedBy_FK]
    foreign key ([CreatedBy]) references [User] ([Id]),
    constraint [Status_UpdatedBy_FK]
    foreign key ([UpdatedBy]) references [User] ([Id]),
)

create table [TrackingLogEntry]
(
    [Id] int primary key identity(1, 1),
    [Title] nvarchar(256) not null,
    [Description] nvarchar(512),
    [TrackingLogId] int not null,
    [StatusId] int not null,
    [Priority] int,
    [OrderIndex] decimal(19,2) not null,
    [CreatedBy] int not null,
    [CreatedAt] datetime2 not null,
    [UpdatedBy] int not null,
    [UpdatedAt] datetime2 not null,

    constraint [TrackingLogEntry_TrackingLogId_FK] 
    foreign key ([TrackingLogId]) references [TrackingLog] ([Id]),
    constraint [TrackingLogEntry_StatusId_FK]
    foreign key ([StatusId]) references [Status] ([Id]),
    constraint [TrackingLogEntry_CreatedBy_FK] 
    foreign key ([CreatedBy]) references [User] ([Id]),
    constraint [TrackingLogEntry_UpdatedBy_FK]
    foreign key ([UpdatedBy]) references [User] ([Id]),
)

insert into [User]([UserName], [Email], [CreatedAt], [UpdatedAt], [PasswordHash], [PasswordSalt])
values ('{IntegrationTestBase.UserName}', '{IntegrationTestBase.Email}', GETUTCDATE(), GETUTCDATE(), 0x0DDDB41591C33FD4CF3ADE187DC85475BC00F1BD29BD15D88456F6BF48C97322938D3C8F9F31476B9F5E1EA983680F08339C8EB9814D37930A17A3A6C4521329, 0x3248D43ABB0866DE81683FC7CDDE1EB2F01852515E208796B2220006CFC5997ABD1A4AA05BE6F35569E73562EB08F8DEE136733BACD41DDBC7E8F39F4A036298962CFEBD72293CF1CBD97EA9F4C107045D112224ECDF8001D276F13B79C527B72ADBC2742D50DAF07C0E02F93D15AD275C59275E173D05D730854C3064563E52)";

        await connection.ExecuteAsync(sql);
    }

    public Task DisposeAsync()
    {
        return MsSqlContainer.DisposeAsync().AsTask();
    }
}