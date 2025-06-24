IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
CREATE TABLE [__EFMigrationsHistory] (
    [MigrationId] nvarchar(150) NOT NULL,
    [ProductVersion] nvarchar(32) NOT NULL,
    CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
GO

CREATE TABLE [User] (
    [Id] int NOT NULL IDENTITY,
    [UserName] nvarchar(256) NOT NULL,
    [FirstName] nvarchar(256) NULL,
    [LastName] nvarchar(256) NULL,
    [Email] nvarchar(256) NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    [PasswordHash] varbinary(256) NOT NULL,
    [PasswordSalt] varbinary(256) NOT NULL,
    CONSTRAINT [User_PK] PRIMARY KEY ([Id])
    );
GO

CREATE TABLE [TrackingLog] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(256) NOT NULL,
    [Description] nvarchar(512) NULL,
    [CreatedBy] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [TrackingLog_PK] PRIMARY KEY ([Id]),
    CONSTRAINT [TrackingLog_CreatedBy_FK] FOREIGN KEY ([CreatedBy]) REFERENCES [User] ([Id]),
    CONSTRAINT [TrackingLog_UpdatedBy_FK] FOREIGN KEY ([UpdatedBy]) REFERENCES [User] ([Id])
    );
GO

CREATE TABLE [TrackingLogEntryStatus] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(256) NOT NULL,
    [Description] nvarchar(512) NULL,
    [TrackingLogId] int NOT NULL,
    [CreatedBy] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [TrackingLogEntryStatus_PK] PRIMARY KEY ([Id]),
    CONSTRAINT [TrackingLogEntryStatus_CreatedBy_FK] FOREIGN KEY ([CreatedBy]) REFERENCES [User] ([Id]),
    CONSTRAINT [TrackingLogEntryStatus_TrackingLogId_FK] FOREIGN KEY ([TrackingLogId]) REFERENCES [TrackingLog] ([Id]),
    CONSTRAINT [TrackingLogEntryStatus_UpdatedBy_FK] FOREIGN KEY ([UpdatedBy]) REFERENCES [User] ([Id])
    );
GO

CREATE TABLE [TrackingLogEntry] (
    [Id] int NOT NULL IDENTITY,
    [Title] nvarchar(256) NOT NULL,
    [Description] nvarchar(512) NULL,
    [TrackingLogId] int NOT NULL,
    [StatusId] int NOT NULL,
    [Priority] int NULL,
    [OrderIndex] decimal(19,2) NOT NULL,
    [CreatedBy] int NOT NULL,
    [CreatedAt] datetime2 NOT NULL,
    [UpdatedBy] int NOT NULL,
    [UpdatedAt] datetime2 NOT NULL,
    CONSTRAINT [TrackingLogEntry_PK] PRIMARY KEY ([Id]),
    CONSTRAINT [TrackingLogEntry_CreatedBy_FK] FOREIGN KEY ([CreatedBy]) REFERENCES [User] ([Id]),
    CONSTRAINT [TrackingLogEntry_StatusId_FK] FOREIGN KEY ([StatusId]) REFERENCES [TrackingLogEntryStatus] ([Id]),
    CONSTRAINT [TrackingLogEntry_TrackingLogId_FK] FOREIGN KEY ([TrackingLogId]) REFERENCES [TrackingLog] ([Id]),
    CONSTRAINT [TrackingLogEntry_UpdatedBy_FK] FOREIGN KEY ([UpdatedBy]) REFERENCES [User] ([Id])
    );
GO

CREATE INDEX [IX_TrackingLog_CreatedBy] ON [TrackingLog] ([CreatedBy]);
GO

CREATE INDEX [IX_TrackingLog_UpdatedBy] ON [TrackingLog] ([UpdatedBy]);
GO

CREATE INDEX [IX_TrackingLogEntry_CreatedBy] ON [TrackingLogEntry] ([CreatedBy]);
GO

CREATE INDEX [IX_TrackingLogEntry_StatusId] ON [TrackingLogEntry] ([StatusId]);
GO

CREATE INDEX [IX_TrackingLogEntry_TrackingLogId] ON [TrackingLogEntry] ([TrackingLogId]);
GO

CREATE INDEX [IX_TrackingLogEntry_UpdatedBy] ON [TrackingLogEntry] ([UpdatedBy]);
GO

CREATE INDEX [IX_TrackingLogEntryStatus_CreatedBy] ON [TrackingLogEntryStatus] ([CreatedBy]);
GO

CREATE INDEX [IX_TrackingLogEntryStatus_TrackingLogId] ON [TrackingLogEntryStatus] ([TrackingLogId]);
GO

CREATE INDEX [IX_TrackingLogEntryStatus_UpdatedBy] ON [TrackingLogEntryStatus] ([UpdatedBy]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20250624182022_Create', N'8.0.17');
GO

COMMIT;
GO