CREATE TABLE [dbo].[DataStoreZ] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreZ] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

