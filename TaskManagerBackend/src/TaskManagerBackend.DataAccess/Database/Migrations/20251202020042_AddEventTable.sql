BEGIN TRANSACTION;
GO

CREATE TABLE [Event] (
    [Id] uniqueidentifier NOT NULL,
    [EntityType] int NOT NULL,
    [EntityId] int NOT NULL,
    [EntityVersion] int NOT NULL,
    [Data] nvarchar(MAX) NOT NULL,
    [DispatchedByUserId] int NOT NULL,
    [DispatchedAt] datetime2 NOT NULL,
    [CorrelationId] uniqueidentifier NOT NULL,
    [CausationId] uniqueidentifier NOT NULL,
    CONSTRAINT [Event_PK] PRIMARY KEY ([Id]),
    CONSTRAINT [Event_CausationId_FK] FOREIGN KEY ([CausationId]) REFERENCES [Event] ([Id]),
    CONSTRAINT [Event_CorrelationId_FK] FOREIGN KEY ([CorrelationId]) REFERENCES [Event] ([Id]),
    CONSTRAINT [Event_DispatchedByUserId_FK] FOREIGN KEY ([DispatchedByUserId]) REFERENCES [User] ([Id]) ON DELETE CASCADE
);
GO

CREATE INDEX [IX_Event_CausationId] ON [Event] ([CausationId]);
GO

CREATE INDEX [IX_Event_CorrelationId] ON [Event] ([CorrelationId]);
GO

CREATE INDEX [IX_Event_DispatchedByUserId] ON [Event] ([DispatchedByUserId]);
GO

CREATE UNIQUE INDEX [IX_Event_EntityType_EntityId_EntityVersion] ON [Event] ([EntityType], [EntityId], [EntityVersion]);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20251202020042_AddEventTable', N'8.0.17');
GO

COMMIT;
GO

