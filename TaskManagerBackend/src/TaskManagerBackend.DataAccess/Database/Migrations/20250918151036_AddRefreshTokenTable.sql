BEGIN TRANSACTION;
GO

CREATE TABLE [RefreshToken] (
    [Id] int NOT NULL IDENTITY,
    [UserId] int NOT NULL,
    [Token] nvarchar(1024) NOT NULL,
    [IssuedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    CONSTRAINT [RefreshToken_PK] PRIMARY KEY ([Id]),
    CONSTRAINT [RefreshToken_UserId_FK] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_RefreshToken_UserId] ON [RefreshToken] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250918151036_AddRefreshTokenTable', N'8.0.17');
GO

COMMIT;
GO

