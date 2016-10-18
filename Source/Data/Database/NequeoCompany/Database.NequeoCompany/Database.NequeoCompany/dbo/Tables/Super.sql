CREATE TABLE [dbo].[Super] (
    [SuperID]     INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]  INT           NOT NULL,
    [SuperFundID] INT           NOT NULL,
    [PaymentDate] DATETIME      NOT NULL,
    [Amount]      MONEY         NOT NULL,
    [Description] VARCHAR (100) NULL,
    [Comments]    VARCHAR (300) NULL,
    CONSTRAINT [PK_Super] PRIMARY KEY CLUSTERED ([SuperID] ASC)
);

