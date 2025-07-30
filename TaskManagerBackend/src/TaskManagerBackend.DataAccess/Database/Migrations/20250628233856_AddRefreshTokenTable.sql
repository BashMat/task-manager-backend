BEGIN TRANSACTION;
GO

CREATE TABLE [RefreshToken] (
    [UserId] int NOT NULL,
    [Token] nvarchar(1024) NOT NULL,
    [IssuedAt] datetime2 NOT NULL,
    [ExpiresAt] datetime2 NOT NULL,
    CONSTRAINT [RefreshToken_PK] PRIMARY KEY ([UserId]),
    CONSTRAINT [RefreshToken_UserId_FK] FOREIGN KEY ([UserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250628233856_AddRefreshTokenTable', N'8.0.17');
GO

COMMIT;
GO