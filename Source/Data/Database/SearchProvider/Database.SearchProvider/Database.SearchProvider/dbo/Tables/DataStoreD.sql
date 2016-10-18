CREATE TABLE [dbo].[DataStoreD] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreD] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

