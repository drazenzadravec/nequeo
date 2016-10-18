CREATE TABLE [dbo].[PaymentType] (
    [PaymentTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]          VARCHAR (50)  NOT NULL,
    [Description]   VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_PaymentType] PRIMARY KEY CLUSTERED ([PaymentTypeID] ASC)
);

