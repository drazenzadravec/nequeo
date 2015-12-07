CREATE TABLE [dbo].[Continent] (
    [ContinentID]          BIGINT        IDENTITY (1, 1) NOT NULL,
    [ContinentName]        VARCHAR (500) NOT NULL,
    [ContinentDescription] VARCHAR (MAX) NULL,
    [ContinentCodeID]      BIGINT        NOT NULL,
    [ContinentGroupOrder]  BIGINT        NOT NULL,
    [ContinentVisible]     BIT           NOT NULL,
    CONSTRAINT [PK_Continent] PRIMARY KEY CLUSTERED ([ContinentID] ASC),
    CONSTRAINT [IX_ContinentCodeID] UNIQUE NONCLUSTERED ([ContinentCodeID] ASC)
);

