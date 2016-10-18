CREATE TABLE [dbo].[Communication_ManageURL_URL] (
    [CommunicationID]           BIGINT         IDENTITY (1, 1) NOT NULL,
    [Communication_ManageURLID] BIGINT         NOT NULL,
    [Service]                   NVARCHAR (MAX) NOT NULL,
    [Value]                     NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Communication_ManageURL_URL] PRIMARY KEY CLUSTERED ([CommunicationID] ASC)
);

