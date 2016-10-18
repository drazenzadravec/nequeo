CREATE TABLE [dbo].[DataStoreH] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreH] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

