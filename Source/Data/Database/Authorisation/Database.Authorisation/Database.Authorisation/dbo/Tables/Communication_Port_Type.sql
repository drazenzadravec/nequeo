CREATE TABLE [dbo].[Communication_Port_Type] (
    [CommunicationID]      BIGINT         IDENTITY (1, 1) NOT NULL,
    [Communication_PortID] BIGINT         NOT NULL,
    [Name]                 NVARCHAR (200) NOT NULL,
    [Number]               INT            NOT NULL,
    [Value]                NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Communication_Port_Type] PRIMARY KEY CLUSTERED ([CommunicationID] ASC)
);

