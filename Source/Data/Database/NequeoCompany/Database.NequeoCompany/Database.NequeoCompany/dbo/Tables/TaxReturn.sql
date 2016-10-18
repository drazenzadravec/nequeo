CREATE TABLE [dbo].[TaxReturn] (
    [TaxReturnID]              INT           IDENTITY (1, 1) NOT NULL,
    [CompanyID]                INT           NOT NULL,
    [TaxReturnDate]            DATETIME      NOT NULL,
    [Amount]                   MONEY         NOT NULL,
    [Description]              VARCHAR (150) NULL,
    [Comments]                 VARCHAR (300) NULL,
    [PaymentDate]              DATETIME      NULL,
    [EFTCodeCustomerReference] VARCHAR (50)  NULL,
    [ATOCode]                  VARCHAR (50)  NULL,
    [ATOFileNumber]            VARCHAR (50)  NULL,
    CONSTRAINT [PK_TaxReturn] PRIMARY KEY CLUSTERED ([TaxReturnID] ASC)
);

