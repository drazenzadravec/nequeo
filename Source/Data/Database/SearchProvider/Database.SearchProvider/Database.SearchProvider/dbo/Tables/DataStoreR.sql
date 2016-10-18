CREATE TABLE [dbo].[DataStoreR] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreR] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

