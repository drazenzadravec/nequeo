CREATE TABLE [dbo].[DataStoreY] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreY] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

