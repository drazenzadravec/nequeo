CREATE TABLE [dbo].[Accounts] (
    [AccountID]     INT           IDENTITY (1, 1) NOT NULL,
    [Firstname]     VARCHAR (40)  NULL,
    [Surname]       VARCHAR (40)  NULL,
    [Bank]          VARCHAR (100) NOT NULL,
    [AccountName]   VARCHAR (60)  NULL,
    [BSB]           VARCHAR (10)  NOT NULL,
    [AccountNumber] VARCHAR (14)  NOT NULL,
    [AccountType]   VARCHAR (50)  NULL,
    [Branch]        VARCHAR (20)  NULL,
    [PhoneNumber]   VARCHAR (14)  NULL,
    [FaxNumber]     VARCHAR (14)  NULL,
    [EmailAddress]  VARCHAR (100) NULL,
    [WebSite]       VARCHAR (100) NULL,
    [Comments]      VARCHAR (300) NULL,
    [Balance]       MONEY         NULL,
    CONSTRAINT [PK_Accounts] PRIMARY KEY CLUSTERED ([AccountID] ASC)
);

