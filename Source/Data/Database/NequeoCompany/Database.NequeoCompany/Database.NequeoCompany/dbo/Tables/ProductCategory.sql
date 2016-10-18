CREATE TABLE [dbo].[ProductCategory] (
    [CategoryID] INT           IDENTITY (1, 1) NOT NULL,
    [Category]   VARCHAR (50)  NOT NULL,
    [Comments]   VARCHAR (500) NULL,
    CONSTRAINT [PK_ProductCategory] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

