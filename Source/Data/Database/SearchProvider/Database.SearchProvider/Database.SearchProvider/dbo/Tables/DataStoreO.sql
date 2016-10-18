CREATE TABLE [dbo].[DataStoreO] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreO] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

