CREATE TABLE [dbo].[Language] (
    [LanguageID]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [LanguageCode]        VARCHAR (50)  NOT NULL,
    [LanguageCountryCode] VARCHAR (50)  NOT NULL,
    [CountryName]         VARCHAR (500) NOT NULL,
    [LanguageName]        VARCHAR (500) NOT NULL,
    [GroupOrder]          BIGINT        NOT NULL,
    [LanguageVisible]     BIT           CONSTRAINT [DF_Language_LanguageVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Language] PRIMARY KEY CLUSTERED ([LanguageID] ASC),
    CONSTRAINT [FK_Language_Country] FOREIGN KEY ([CountryName]) REFERENCES [dbo].[Country] ([CountryName])
);

