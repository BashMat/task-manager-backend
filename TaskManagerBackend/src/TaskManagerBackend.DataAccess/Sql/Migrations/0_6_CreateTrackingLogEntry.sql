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