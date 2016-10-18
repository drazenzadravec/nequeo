CREATE TABLE [dbo].[Communication_Port] (
    [CommunicationID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [ServiceName]     NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Communication_Port] PRIMARY KEY CLUSTERED ([CommunicationID] ASC)
);

