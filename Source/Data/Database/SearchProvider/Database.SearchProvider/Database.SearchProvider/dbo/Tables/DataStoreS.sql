CREATE TABLE [dbo].[DataStoreS] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreS] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

