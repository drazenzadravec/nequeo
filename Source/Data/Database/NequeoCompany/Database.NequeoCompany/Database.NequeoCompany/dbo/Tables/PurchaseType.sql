CREATE TABLE [dbo].[PurchaseType] (
    [PurchaseTypeID] INT          IDENTITY (1, 1) NOT NULL,
    [Name]           VARCHAR (50) NOT NULL,
    [Description]    VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_PurchaseType] PRIMARY KEY CLUSTERED ([PurchaseTypeID] ASC)
);

