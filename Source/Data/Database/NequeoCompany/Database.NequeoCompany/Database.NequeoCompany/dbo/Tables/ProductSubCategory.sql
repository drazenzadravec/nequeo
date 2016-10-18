CREATE TABLE [dbo].[ProductSubCategory] (
    [SubCategoryID]   INT           IDENTITY (1, 1) NOT NULL,
    [CategoryID]      INT           NOT NULL,
    [SubCategoryName] VARCHAR (100) NOT NULL,
    [Comments]        VARCHAR (500) NULL,
    CONSTRAINT [PK_ProductSubCategory] PRIMARY KEY CLUSTERED ([SubCategoryID] ASC)
);

