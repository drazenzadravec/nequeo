CREATE TABLE [dbo].[Wages] (
    [WageID]      INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]  INT           NOT NULL,
    [AccountID]   INT           NOT NULL,
    [PaymentDate] DATETIME      NOT NULL,
    [Hours]       FLOAT (53)    NOT NULL,
    [Rate]        MONEY         NOT NULL,
    [Amount]      MONEY         NOT NULL,
    [PAYG]        MONEY         NOT NULL,
    [Description] VARCHAR (50)  NULL,
    [Comments]    VARCHAR (100) NULL,
    CONSTRAINT [PK_Wages] PRIMARY KEY CLUSTERED ([WageID] ASC)
);

