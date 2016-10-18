CREATE TABLE [dbo].[DataStoreM] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreM] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

