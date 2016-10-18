CREATE TABLE [dbo].[DataStoreU] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreU] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

