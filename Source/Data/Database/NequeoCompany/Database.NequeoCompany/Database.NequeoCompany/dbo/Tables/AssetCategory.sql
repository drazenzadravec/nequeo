CREATE TABLE [dbo].[AssetCategory] (
    [CategoryID] INT           IDENTITY (1, 1) NOT NULL,
    [Category]   VARCHAR (50)  NOT NULL,
    [Comments]   VARCHAR (500) NULL,
    CONSTRAINT [PK_AssetCategory] PRIMARY KEY CLUSTERED ([CategoryID] ASC)
);

