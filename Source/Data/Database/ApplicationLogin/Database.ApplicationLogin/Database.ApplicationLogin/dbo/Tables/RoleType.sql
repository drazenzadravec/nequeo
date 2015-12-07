CREATE TABLE [dbo].[RoleType] (
    [RoleTypeID]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [RoleTypeName]        VARCHAR (100) NOT NULL,
    [RoleTypeDescription] VARCHAR (500) NOT NULL,
    [RoleTypeCodeID]      BIGINT        NOT NULL,
    [RoleTypeGroupOrder]  BIGINT        NOT NULL,
    [RoleTypeVisible]     BIT           CONSTRAINT [DF_RoleType_RoleTypeVisible] DEFAULT ((1)) NOT NULL,
    [ModifiedDate]        DATETIME      NULL,
    [RowVersionData]      ROWVERSION    NULL,
    CONSTRAINT [PK_RoleType] PRIMARY KEY CLUSTERED ([RoleTypeID] ASC),
    CONSTRAINT [IX_RoleTypeCodeID] UNIQUE NONCLUSTERED ([RoleTypeCodeID] ASC)
);

