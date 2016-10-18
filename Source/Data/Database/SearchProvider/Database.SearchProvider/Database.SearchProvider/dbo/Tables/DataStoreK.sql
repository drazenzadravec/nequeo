CREATE TABLE [dbo].[DataStoreK] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreK] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

