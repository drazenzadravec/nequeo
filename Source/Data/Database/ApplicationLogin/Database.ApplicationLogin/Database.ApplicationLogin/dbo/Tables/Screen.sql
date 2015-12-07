CREATE TABLE [dbo].[Screen] (
    [ScreenID]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [ApplicationID]     BIGINT        NOT NULL,
    [ScreenName]        VARCHAR (50)  NOT NULL,
    [ScreenDescription] VARCHAR (500) NOT NULL,
    [ScreenCodeID]      BIGINT        NOT NULL,
    [ScreenGroupOrder]  BIGINT        NOT NULL,
    [ScreenVisible]     BIT           CONSTRAINT [DF_Screen_ScreenVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Screen] PRIMARY KEY CLUSTERED ([ScreenID] ASC),
    CONSTRAINT [FK_Screen_Application] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Application] ([ApplicationID]),
    CONSTRAINT [IX_ScreenApplicationGroup] UNIQUE NONCLUSTERED ([ApplicationID] ASC, [ScreenCodeID] ASC)
);

