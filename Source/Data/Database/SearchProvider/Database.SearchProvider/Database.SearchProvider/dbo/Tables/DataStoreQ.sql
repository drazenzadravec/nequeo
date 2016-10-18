CREATE TABLE [dbo].[DataStoreQ] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreQ] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

