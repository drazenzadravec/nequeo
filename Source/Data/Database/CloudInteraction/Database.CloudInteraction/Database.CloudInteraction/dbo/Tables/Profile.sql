CREATE TABLE [dbo].[Profile] (
    [ProfileID]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [Username]         VARCHAR (100)  NOT NULL,
    [ApplicationName]  NVARCHAR (300) NOT NULL,
    [IsAnonymous]      BIT            CONSTRAINT [DF_Profile_IsAnonymous] DEFAULT ((0)) NULL,
    [LastActivityDate] DATETIME       CONSTRAINT [DF_Profile_LastActivityDate] DEFAULT (getdate()) NULL,
    [LastUpdatedDate]  DATETIME       CONSTRAINT [DF_Profile_LastUpdatedDate] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_Profile] PRIMARY KEY CLUSTERED ([ProfileID] ASC),
    CONSTRAINT [IX_ProfileUsernameApplicationName] UNIQUE NONCLUSTERED ([Username] ASC, [ApplicationName] ASC)
);

