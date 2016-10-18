CREATE TABLE [dbo].[DataStoreX] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreX] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

