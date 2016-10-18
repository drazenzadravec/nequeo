CREATE TABLE [dbo].[InvoiceProducts] (
    [InvoiceProductID] INT           IDENTITY (1, 1) NOT NULL,
    [InvoiceID]        INT           NOT NULL,
    [ProductID]        INT           NOT NULL,
    [Units]            INT           NOT NULL,
    [Description]      VARCHAR (500) NOT NULL,
    [UnitPrice]        MONEY         NOT NULL,
    [Total]            MONEY         NOT NULL,
    [GST]              MONEY         NOT NULL,
    [Comments]         VARCHAR (500) NULL,
    CONSTRAINT [PK_InvoiceProducts] PRIMARY KEY CLUSTERED ([InvoiceProductID] ASC)
);

