CREATE TABLE [dbo].[Customers] (
    [CustomerID]     INT           IDENTITY (1, 1) NOT NULL,
    [CompanyName]    VARCHAR (200) NOT NULL,
    [Firstname]      VARCHAR (40)  NULL,
    [Surname]        VARCHAR (40)  NULL,
    [Address]        VARCHAR (50)  NULL,
    [Suburb]         VARCHAR (30)  NULL,
    [State]          VARCHAR (5)   NULL,
    [PostCode]       VARCHAR (5)   NULL,
    [PhoneNumber]    VARCHAR (14)  NULL,
    [FaxNumber]      VARCHAR (14)  NULL,
    [MobileNumber]   VARCHAR (12)  NULL,
    [EmailAddress]   VARCHAR (100) NULL,
    [WebSite]        VARCHAR (100) NULL,
    [ABN]            VARCHAR (15)  NULL,
    [PostalAddress]  VARCHAR (50)  NULL,
    [PostalSuburb]   VARCHAR (30)  NULL,
    [PostalState]    VARCHAR (5)   NULL,
    [PostalPostCode] VARCHAR (5)   NULL,
    [Comments]       VARCHAR (500) NULL,
    CONSTRAINT [PK_Customers] PRIMARY KEY CLUSTERED ([CustomerID] ASC)
);

