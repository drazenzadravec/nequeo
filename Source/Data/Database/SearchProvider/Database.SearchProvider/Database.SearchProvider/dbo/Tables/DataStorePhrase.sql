CREATE TABLE [dbo].[DataStorePhrase] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStorePhrase] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

