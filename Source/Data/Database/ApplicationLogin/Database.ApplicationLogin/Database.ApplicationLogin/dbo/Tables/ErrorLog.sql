CREATE TABLE [dbo].[ErrorLog] (
    [ErrorLogID]       BIGINT        IDENTITY (1, 1) NOT NULL,
    [ApplicationID]    BIGINT        NOT NULL,
    [ProcessName]      VARCHAR (200) NULL,
    [ErrorCode]        VARCHAR (50)  NULL,
    [ErrorDescription] VARCHAR (MAX) NULL,
    [ErrorDate]        DATETIME      CONSTRAINT [DF_ErrorLog_ErrorDate] DEFAULT (getdate()) NULL,
    CONSTRAINT [PK_ErrorLog] PRIMARY KEY CLUSTERED ([ErrorLogID] ASC)
);

