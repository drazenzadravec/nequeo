CREATE TABLE [dbo].[IncomeType] (
    [IncomeTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Description]   VARCHAR (100) NOT NULL,
    [Name]         VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_IncomeType] PRIMARY KEY CLUSTERED ([IncomeTypeID] ASC)
);

