CREATE TABLE [dbo].[DataStoreB] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreB] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

