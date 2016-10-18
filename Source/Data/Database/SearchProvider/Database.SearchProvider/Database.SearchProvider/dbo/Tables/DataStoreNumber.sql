CREATE TABLE [dbo].[DataStoreNumber] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreNumber] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

