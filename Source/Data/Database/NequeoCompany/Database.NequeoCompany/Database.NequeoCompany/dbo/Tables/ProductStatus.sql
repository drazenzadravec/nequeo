CREATE TABLE [dbo].[ProductStatus] (
    [ProductStatusID] INT           IDENTITY (1, 1) NOT NULL,
    [StatusName]      VARCHAR (100) NOT NULL,
    [Description]     VARCHAR (MAX) NULL,
    CONSTRAINT [PK_ProductStatus] PRIMARY KEY CLUSTERED ([ProductStatusID] ASC)
);

