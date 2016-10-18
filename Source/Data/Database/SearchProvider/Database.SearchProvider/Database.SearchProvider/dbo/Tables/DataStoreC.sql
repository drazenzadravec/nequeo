CREATE TABLE [dbo].[DataStoreC] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreC] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

