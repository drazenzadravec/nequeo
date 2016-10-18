CREATE TABLE [dbo].[Assets] (
    [AssetID]             INT           IDENTITY (1, 1) NOT NULL,
    [VendorID]            INT           NULL,
    [VendorDetailsID]     INT           NULL,
    [Location]            VARCHAR (150) NULL,
    [Model]               VARCHAR (200) NULL,
    [SerialNumber]        VARCHAR (50)  NULL,
    [Manufacturer]        VARCHAR (150) NULL,
    [ManufacturerWebSite] VARCHAR (200) NULL,
    [Category]            VARCHAR (50)  NULL,
    [Units]               INT           NULL,
    [Status]              VARCHAR (20)  NULL,
    [Description]         VARCHAR (300) NULL,
    [Details]             VARCHAR (500) NULL,
    [Comments]            VARCHAR (500) NULL,
    [Amount]              MONEY         NULL,
    CONSTRAINT [PK_Assets] PRIMARY KEY CLUSTERED ([AssetID] ASC)
);

