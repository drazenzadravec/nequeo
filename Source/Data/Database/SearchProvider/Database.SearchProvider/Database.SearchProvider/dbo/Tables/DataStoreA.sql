CREATE TABLE [dbo].[DataStoreA] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreA] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

