CREATE TABLE [dbo].[BAS] (
    [BASID]               INT           IDENTITY (1, 1) NOT NULL,
    [CompanyID]           INT           NOT NULL,
    [BasDate]             DATETIME      NOT NULL,
    [DocumentID]          VARCHAR (20)  NULL,
    [Reference]           VARCHAR (20)  NULL,
    [EFTCode]             VARCHAR (20)  NULL,
    [TotalSales]          MONEY         NOT NULL,
    [ExportSales]         MONEY         NULL,
    [OtherGSTFreeSales]   MONEY         NULL,
    [CapitalPurchases]    MONEY         NULL,
    [NonCapitalPurchases] MONEY         NOT NULL,
    [GSTOnSales]          MONEY         NOT NULL,
    [GSTOnPurchases]      MONEY         NOT NULL,
    [PaymentOrRefund]     VARCHAR (10)  NOT NULL,
    [Amount]              MONEY         NOT NULL,
    [Description]         VARCHAR (150) NULL,
    [Comments]            VARCHAR (300) NULL,
    [TotalWages]          MONEY         NOT NULL,
    [PAYGWithheld]        MONEY         NOT NULL,
    CONSTRAINT [PK_BAS] PRIMARY KEY CLUSTERED ([BASID] ASC)
);

