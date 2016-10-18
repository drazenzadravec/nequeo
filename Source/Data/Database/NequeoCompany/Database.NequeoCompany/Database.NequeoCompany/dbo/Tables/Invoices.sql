CREATE TABLE [dbo].[Invoices] (
    [InvoiceID]   INT           IDENTITY (1, 1) NOT NULL,
    [CustomerID]  INT           NOT NULL,
    [InvoiceDate] DATETIME      NOT NULL,
    [PaymentDate] DATETIME      NULL,
    [OrderID]     VARCHAR (20)  NOT NULL,
    [Developer]   VARCHAR (40)  NULL,
    [GSTIncluded] VARCHAR (10)  NULL,
    [Comments]    VARCHAR (500) NULL,
    [IncomeType]  VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_Invoices] PRIMARY KEY CLUSTERED ([InvoiceID] ASC)
);

