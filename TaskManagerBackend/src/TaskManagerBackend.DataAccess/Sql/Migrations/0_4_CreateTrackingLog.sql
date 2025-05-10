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