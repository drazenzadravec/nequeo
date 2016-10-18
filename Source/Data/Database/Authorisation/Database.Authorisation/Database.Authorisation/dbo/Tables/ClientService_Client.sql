CREATE TABLE [dbo].[ClientService_Client] (
    [ClientServiceID]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [UniqueIdentifier] NVARCHAR (100) NOT NULL,
    [ServiceName]      NVARCHAR (100) NOT NULL,
    [Host]             NVARCHAR (100) NOT NULL,
    [Active]           BIT            NOT NULL,
    [DateAdded]        DATETIME       NOT NULL,
    [Value]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ClientService_Client] PRIMARY KEY CLUSTERED ([ClientServiceID] ASC)
);

