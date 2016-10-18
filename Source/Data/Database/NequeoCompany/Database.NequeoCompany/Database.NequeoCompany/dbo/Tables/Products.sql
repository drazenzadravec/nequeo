CREATE TABLE [dbo].[Products] (
    [ProductID]      INT           IDENTITY (1, 1) NOT NULL,
    [ProductNumber]  VARCHAR (50)  NULL,
    [ProductVersion] VARCHAR (50)  NULL,
    [Description]    VARCHAR (200) NOT NULL,
    [ProductionDate] DATETIME      NULL,
    [Model]          VARCHAR (100) NULL,
    [SerialNumber]   VARCHAR (50)  NULL,
    [Category]       VARCHAR (50)  NULL,
    [Units]          INT           NULL,
    [UnitPrice]      MONEY         NOT NULL,
    [Status]         VARCHAR (50)  NULL,
    [Comments]       VARCHAR (500) NULL,
    [SubCategory]    VARCHAR (50)  NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY CLUSTERED ([ProductID] ASC)
);

