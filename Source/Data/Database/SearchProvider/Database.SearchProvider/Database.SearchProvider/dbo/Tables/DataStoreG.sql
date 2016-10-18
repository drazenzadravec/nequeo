CREATE TABLE [dbo].[DataStoreG] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreG] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

