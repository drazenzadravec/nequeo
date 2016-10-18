CREATE TABLE [dbo].[DataStoreJ] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreJ] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

