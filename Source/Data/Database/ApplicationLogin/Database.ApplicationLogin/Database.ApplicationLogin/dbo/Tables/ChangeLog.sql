CREATE TABLE [dbo].[ChangeLog] (
    [ChangeID]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserID]        BIGINT        NOT NULL,
    [ScreenID]      BIGINT        NOT NULL,
    [ApplicationID] BIGINT        NOT NULL,
    [PrimaryKeyID]  VARCHAR (200) NOT NULL,
    [ModifiedDate]  DATETIME      NOT NULL,
    [Change]        TEXT          NOT NULL,
    [IPAddress]     VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_ChangeLog] PRIMARY KEY CLUSTERED ([ChangeID] ASC),
    CONSTRAINT [FK_ChangeLog_Application] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Application] ([ApplicationID]),
    CONSTRAINT [FK_ChangeLog_Screen] FOREIGN KEY ([ScreenID]) REFERENCES [dbo].[Screen] ([ScreenID])
);

