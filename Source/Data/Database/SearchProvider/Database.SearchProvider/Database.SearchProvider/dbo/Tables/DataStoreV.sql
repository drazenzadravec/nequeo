CREATE TABLE [dbo].[DataStoreV] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreV] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

