create table [Card]
(
    [Id] int primary key identity(1, 1),
    [Title] nvarchar(256) not null,
    [Description] nvarchar(512),
    [ColumnId] int,
    [OrderIndex] int not null,
    [CreatedBy] int,
    [CreatedAt] datetime2 not null,
    [UpdatedBy] int,
    [UpdatedAt] datetime2 not null,

    constraint [Card_ColumnId_FK] 
    foreign key ([ColumnId]) references [Column] ([Id]),
    constraint [Card_CreatedBy_FK] 
    foreign key ([CreatedBy]) references [User] ([Id]),
    constraint [Card_UpdatedBy_FK]
    foreign key ([UpdatedBy]) references [User] ([Id]),
)