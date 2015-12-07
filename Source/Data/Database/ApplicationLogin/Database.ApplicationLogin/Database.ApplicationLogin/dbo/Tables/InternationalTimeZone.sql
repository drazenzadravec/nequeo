CREATE TABLE [dbo].[InternationalTimeZone] (
    [InternationalTimeZoneID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [SystemTimeZoneName]      VARCHAR (200) NOT NULL,
    [DisplayTimeZoneName]     VARCHAR (200) NOT NULL,
    [UniversalTimeOffset]     VARCHAR (50)  NULL,
    [GroupOrder]              BIGINT        NOT NULL,
    [TimeZoneVisible]         BIT           CONSTRAINT [DF_InternationalTimeZone_TimeZoneVisible] DEFAULT ((1)) NOT NULL,
    [CountryName]             VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_InternationalTimeZone] PRIMARY KEY CLUSTERED ([InternationalTimeZoneID] ASC),
    CONSTRAINT [FK_InternationalTimeZone_Country] FOREIGN KEY ([CountryName]) REFERENCES [dbo].[Country] ([CountryName])
);

