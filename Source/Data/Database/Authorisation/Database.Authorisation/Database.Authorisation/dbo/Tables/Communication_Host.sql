CREATE TABLE [dbo].[Communication_Host] (
    [CommunicationID]   BIGINT         IDENTITY (1, 1) NOT NULL,
    [Name]              NVARCHAR (200) NOT NULL,
    [Index]             BIGINT         NOT NULL,
    [ActiveConnections] BIGINT         NOT NULL,
    [Domain]            NVARCHAR (200) NOT NULL,
    [Type]              NVARCHAR (200) NOT NULL,
    [Value]             NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Communication_Host] PRIMARY KEY CLUSTERED ([CommunicationID] ASC)
);

