CREATE TABLE [dbo].[DataStoreW] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreW] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

