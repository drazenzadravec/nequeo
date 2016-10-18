CREATE TABLE [dbo].[EmployeeBankAccounts] (
    [AccountID]     INT           IDENTITY (1, 1) NOT NULL,
    [EmployeeID]    INT           NOT NULL,
    [Bank]          VARCHAR (50)  NOT NULL,
    [AccountName]   VARCHAR (50)  NOT NULL,
    [BSB]           VARCHAR (8)   NOT NULL,
    [AccountNumber] VARCHAR (15)  NOT NULL,
    [PhoneNumber]   VARCHAR (15)  NULL,
    [WebSite]       VARCHAR (100) NULL,
    [Comments]      VARCHAR (100) NULL,
    CONSTRAINT [PK_EmployeeAccounts] PRIMARY KEY CLUSTERED ([AccountID] ASC)
);

