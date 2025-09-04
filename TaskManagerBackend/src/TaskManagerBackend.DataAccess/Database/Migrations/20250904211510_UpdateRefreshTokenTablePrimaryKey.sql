BEGIN TRANSACTION;
GO

DROP INDEX [IX_RefreshToken_UserId] ON [RefreshToken];
GO

CREATE INDEX [IX_RefreshToken_UserId] ON [RefreshToken] ([UserId]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250904211510_UpdateRefreshTokenTablePrimaryKey', N'8.0.17');
GO

COMMIT;
GO