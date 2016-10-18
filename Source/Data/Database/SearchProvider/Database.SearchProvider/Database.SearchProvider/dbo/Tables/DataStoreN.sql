CREATE TABLE [dbo].[DataStoreN] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreN] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

