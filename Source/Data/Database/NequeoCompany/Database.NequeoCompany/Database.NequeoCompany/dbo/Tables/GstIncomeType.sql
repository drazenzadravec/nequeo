CREATE TABLE [dbo].[GstIncomeType] (
    [GstIncomeTypeID] INT          IDENTITY (1, 1) NOT NULL,
    [Name]            VARCHAR (50) NOT NULL,
    CONSTRAINT [PK_GstIncomeType] PRIMARY KEY CLUSTERED ([GstIncomeTypeID] ASC)
);

