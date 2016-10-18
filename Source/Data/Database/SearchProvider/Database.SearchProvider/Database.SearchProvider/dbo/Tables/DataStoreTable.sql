CREATE TABLE [dbo].[DataStoreTable] (
    [DataStoreTableID]   BIGINT       IDENTITY (1, 1) NOT NULL,
    [DataStoreTableName] VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_DataStoreTable] PRIMARY KEY CLUSTERED ([DataStoreTableID] ASC)
);

