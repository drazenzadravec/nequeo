CREATE TABLE [dbo].[Communication_ManageURL] (
    [CommunicationID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [Type]            NVARCHAR (200) NOT NULL,
    CONSTRAINT [PK_Communication_ManageURL] PRIMARY KEY CLUSTERED ([CommunicationID] ASC)
);

