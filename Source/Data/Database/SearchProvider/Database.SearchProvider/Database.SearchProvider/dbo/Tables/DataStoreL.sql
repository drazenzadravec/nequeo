CREATE TABLE [dbo].[DataStoreL] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreL] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

