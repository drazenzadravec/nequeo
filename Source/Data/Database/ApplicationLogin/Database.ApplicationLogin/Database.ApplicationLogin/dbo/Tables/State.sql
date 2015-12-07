CREATE TABLE [dbo].[State] (
    [StateID]        BIGINT        IDENTITY (1, 1) NOT NULL,
    [CountryID]      BIGINT        NOT NULL,
    [StateShortName] VARCHAR (50)  NOT NULL,
    [StateLongName]  VARCHAR (100) NOT NULL,
    [StateCodeID]    BIGINT        NOT NULL,
    [GroupOrder]     BIGINT        NOT NULL,
    [StateVisible]   BIT           CONSTRAINT [DF_State_StateVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([StateID] ASC),
    CONSTRAINT [FK_State_Country] FOREIGN KEY ([CountryID]) REFERENCES [dbo].[Country] ([CountryID]),
    CONSTRAINT [IX_CountryStateCodeID] UNIQUE NONCLUSTERED ([CountryID] ASC, [StateCodeID] ASC)
);

