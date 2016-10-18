CREATE TABLE [dbo].[DataStoreE] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreE] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

