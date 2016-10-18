CREATE TABLE [dbo].[EmployeeSuperAccounts] (
    [SuperFundID]  INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]   INT           NOT NULL,
    [SuperName]    VARCHAR (100) NOT NULL,
    [PolicyNumber] VARCHAR (15)  NOT NULL,
    [PhoneNumber]  VARCHAR (15)  NULL,
    [WebSite]      VARCHAR (100) NULL,
    [Comments]     VARCHAR (100) NULL,
    CONSTRAINT [PK_EmployeeSuperAccounts] PRIMARY KEY CLUSTERED ([SuperFundID] ASC)
);

