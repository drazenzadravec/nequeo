CREATE TABLE [dbo].[DataStoreI] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreI] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

