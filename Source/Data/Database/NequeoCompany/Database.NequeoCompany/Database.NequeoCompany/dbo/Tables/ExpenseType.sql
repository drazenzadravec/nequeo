CREATE TABLE [dbo].[ExpenseType] (
    [ExpenseTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Description]   VARCHAR (100) NOT NULL,
    [Name]          VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_ExpenseType] PRIMARY KEY CLUSTERED ([ExpenseTypeID] ASC)
);

