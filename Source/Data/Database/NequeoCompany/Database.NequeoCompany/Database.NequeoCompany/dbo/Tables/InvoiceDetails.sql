CREATE TABLE [dbo].[InvoiceDetails] (
    [InvoiceDetailsID] INT           IDENTITY (1, 1) NOT NULL,
    [InvoiceID]        INT           NOT NULL,
    [Hours]            FLOAT (53)    NOT NULL,
    [Description]      VARCHAR (500) NOT NULL,
    [Rate]             MONEY         NOT NULL,
    [Total]            MONEY         NOT NULL,
    [GST]              MONEY         NOT NULL,
    [Comments]         VARCHAR (500) NULL,
    CONSTRAINT [PK_InvoiceDetails] PRIMARY KEY CLUSTERED ([InvoiceDetailsID] ASC)
);

