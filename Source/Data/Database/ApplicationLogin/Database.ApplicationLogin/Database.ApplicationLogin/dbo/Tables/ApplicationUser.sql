CREATE TABLE [dbo].[ApplicationUser] (
    [ApplicationUserID] BIGINT IDENTITY (1, 1) NOT NULL,
    [ApplicationID]     BIGINT NOT NULL,
    [UserID]            BIGINT NOT NULL,
    [RoleTypeID]        BIGINT NOT NULL,
    [Suspended]         BIT    CONSTRAINT [DF_ApplicationUser_Suspended] DEFAULT ((0)) NOT NULL,
    CONSTRAINT [PK_ApplicationUser] PRIMARY KEY CLUSTERED ([ApplicationUserID] ASC),
    CONSTRAINT [FK_ApplicationUser_Application] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Application] ([ApplicationID]),
    CONSTRAINT [FK_ApplicationUser_RoleType] FOREIGN KEY ([RoleTypeID]) REFERENCES [dbo].[RoleType] ([RoleTypeID]),
    CONSTRAINT [IX_ApplicationUserGroup] UNIQUE NONCLUSTERED ([ApplicationID] ASC, [UserID] ASC)
);

