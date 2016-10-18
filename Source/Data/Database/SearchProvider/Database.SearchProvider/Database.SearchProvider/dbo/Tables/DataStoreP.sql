CREATE TABLE [dbo].[DataStoreP] (
    [DataStoreID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [SearchText]  NVARCHAR (MAX) NOT NULL,
    CONSTRAINT [PK_DataStoreP] PRIMARY KEY CLUSTERED ([DataStoreID] ASC)
);

