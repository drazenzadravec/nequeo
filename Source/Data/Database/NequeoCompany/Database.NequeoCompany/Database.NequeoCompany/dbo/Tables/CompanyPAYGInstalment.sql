CREATE TABLE [dbo].[CompanyPAYGInstalment] (
    [PAYGInstID]         INT           IDENTITY (1, 1) NOT NULL,
    [CompanyID]          INT           NOT NULL,
    [AssessmentYear]     VARCHAR (50)  NULL,
    [InstalRate]         FLOAT (53)    NULL,
    [TaxOnIncome]        MONEY         NULL,
    [GDPTax]             MONEY         NULL,
    [InstalCalTaxOffice] MONEY         NULL,
    [InstalmentDate]     DATETIME      NULL,
    [Comments]           VARCHAR (500) NULL,
    CONSTRAINT [PK_CompanyPAYGInstalment] PRIMARY KEY CLUSTERED ([PAYGInstID] ASC)
);

