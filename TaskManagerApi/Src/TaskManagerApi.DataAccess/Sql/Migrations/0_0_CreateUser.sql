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