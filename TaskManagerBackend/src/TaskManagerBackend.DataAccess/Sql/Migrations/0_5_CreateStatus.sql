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