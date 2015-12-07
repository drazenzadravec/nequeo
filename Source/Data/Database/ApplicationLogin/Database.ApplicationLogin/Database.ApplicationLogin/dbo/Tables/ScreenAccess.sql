CREATE TABLE [dbo].[ScreenAccess] (
    [AccessID]     BIGINT IDENTITY (1, 1) NOT NULL,
    [RoleTypeID]   BIGINT NOT NULL,
    [ScreenID]     BIGINT NOT NULL,
    [AccessTypeID] BIGINT NOT NULL,
    CONSTRAINT [PK_Access] PRIMARY KEY CLUSTERED ([AccessID] ASC),
    CONSTRAINT [FK_Access_AccessType] FOREIGN KEY ([AccessTypeID]) REFERENCES [dbo].[AccessType] ([AccessTypeID]),
    CONSTRAINT [FK_Access_RoleType] FOREIGN KEY ([RoleTypeID]) REFERENCES [dbo].[RoleType] ([RoleTypeID]),
    CONSTRAINT [FK_Access_Screen] FOREIGN KEY ([ScreenID]) REFERENCES [dbo].[Screen] ([ScreenID]),
    CONSTRAINT [IX_ScreenAccessGroup] UNIQUE NONCLUSTERED ([RoleTypeID] ASC, [ScreenID] ASC)
);

