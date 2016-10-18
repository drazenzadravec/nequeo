CREATE TABLE [dbo].[DataStoreT] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreT] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

