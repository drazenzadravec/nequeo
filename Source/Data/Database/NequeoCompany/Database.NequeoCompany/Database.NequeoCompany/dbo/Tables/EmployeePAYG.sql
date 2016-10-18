CREATE TABLE [dbo].[EmployeePAYG] (
    [PAYGID]      INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]  INT           NOT NULL,
    [PaymentDate] DATETIME      NOT NULL,
    [Amount]      MONEY         NOT NULL,
    [Description] VARCHAR (100) NULL,
    [Comments]    VARCHAR (300) NULL,
    CONSTRAINT [PK_EmployeePAYG] PRIMARY KEY CLUSTERED ([PAYGID] ASC)
);

