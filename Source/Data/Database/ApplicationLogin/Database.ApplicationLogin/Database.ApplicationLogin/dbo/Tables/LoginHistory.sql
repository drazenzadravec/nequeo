CREATE TABLE [dbo].[LoginHistory] (
    [LoginHistoryID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [UserID]         BIGINT        NOT NULL,
    [ApplicationID]  BIGINT        NOT NULL,
    [LoginDate]      DATETIME      NOT NULL,
    [LogoutDate]     DATETIME      NULL,
    [IPAddress]      VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_LoginHistory] PRIMARY KEY CLUSTERED ([LoginHistoryID] ASC),
    CONSTRAINT [FK_LoginHistory_Application] FOREIGN KEY ([ApplicationID]) REFERENCES [dbo].[Application] ([ApplicationID])
);

