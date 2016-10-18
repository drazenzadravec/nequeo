CREATE TABLE [dbo].[DataStoreF] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreF] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

