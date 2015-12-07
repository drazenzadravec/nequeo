CREATE TABLE [dbo].[Role] (
    [RoleID]          BIGINT         NOT NULL,
    [RoleName]        NVARCHAR (50)  NOT NULL,
    [RoleDescription] NVARCHAR (500) NULL,
    [ApplicationName] NVARCHAR (300) NOT NULL,
    CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED ([RoleID] ASC),
    CONSTRAINT [IX_RolenameApplicationName] UNIQUE NONCLUSTERED ([RoleName] ASC, [ApplicationName] ASC)
);

