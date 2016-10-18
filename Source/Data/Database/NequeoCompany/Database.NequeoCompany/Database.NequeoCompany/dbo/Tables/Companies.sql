CREATE TABLE [dbo].[Companies] (
    [CompanyID]      INT           IDENTITY (1, 1) NOT NULL,
    [CompanyName]    VARCHAR (200) NOT NULL,
    [Firstname]      VARCHAR (40)  NULL,
    [Surname]        VARCHAR (40)  NULL,
    [Address]        VARCHAR (50)  NULL,
    [Suburb]         VARCHAR (30)  NULL,
    [State]          VARCHAR (5)   NULL,
    [Postcode]       VARCHAR (5)   NULL,
    [PhoneNumber]    VARCHAR (14)  NULL,
    [FaxNumber]      VARCHAR (14)  NULL,
    [MobileNumber]   VARCHAR (12)  NULL,
    [EmailAddress]   VARCHAR (100) NULL,
    [WebSite]        VARCHAR (100) NULL,
    [ABN]            VARCHAR (15)  NULL,
    [TFN]            VARCHAR (16)  NULL,
    [PostalAddress]  VARCHAR (50)  NULL,
    [PostalSuburb]   VARCHAR (30)  NULL,
    [PostalState]    VARCHAR (5)   NULL,
    [PostalPostcode] VARCHAR (5)   NULL,
    [Comments]       VARCHAR (500) NULL,
    CONSTRAINT [PK_Misc] PRIMARY KEY CLUSTERED ([CompanyID] ASC)
);

