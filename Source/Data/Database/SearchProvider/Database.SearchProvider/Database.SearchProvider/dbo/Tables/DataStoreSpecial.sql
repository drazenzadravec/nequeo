CREATE TABLE [dbo].[DataStoreSpecial] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreSpecial] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

