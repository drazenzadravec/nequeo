CREATE TABLE [dbo].[Country] (
    [CountryID]       BIGINT        IDENTITY (1, 1) NOT NULL,
    [ContinentID]     BIGINT        NULL,
    [CountryCode]     VARCHAR (50)  NOT NULL,
    [CountryName]     VARCHAR (500) NOT NULL,
    [CountryDialCode] VARCHAR (50)  NULL,
    [GroupOrder]      BIGINT        NOT NULL,
    [CountryVisible]  BIT           CONSTRAINT [DF_Country_CountryVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Country] PRIMARY KEY CLUSTERED ([CountryID] ASC),
    CONSTRAINT [IX_CountryName] UNIQUE NONCLUSTERED ([CountryName] ASC)
);

