CREATE TABLE [dbo].[VendorDetails] (
    [VendorDetailsID] INT           IDENTITY (1, 1) NOT NULL,
    [VendorID]        INT           NOT NULL,
    [PaymentDate]     DATETIME      NULL,
    [InvoiceNumber]   VARCHAR (50)  NULL,
    [OrderNumber]     VARCHAR (50)  NULL,
    [Reference]       VARCHAR (50)  NULL,
    [Amount]          MONEY         NOT NULL,
    [GST]             MONEY         NOT NULL,
    [Description]     VARCHAR (100) NULL,
    [Comments]        VARCHAR (300) NULL,
    [PurchaseType]    VARCHAR (30)  NOT NULL,
    [ExpenseType]     VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_VendorDetails] PRIMARY KEY CLUSTERED ([VendorDetailsID] ASC)
);

