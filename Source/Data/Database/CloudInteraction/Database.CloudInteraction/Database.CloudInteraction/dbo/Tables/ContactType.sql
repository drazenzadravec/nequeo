CREATE TABLE [dbo].[ContactType] (
    [ContactTypeID] BIGINT        IDENTITY (1, 1) NOT NULL,
    [CodeID]        BIGINT        NOT NULL,
    [Name]          VARCHAR (50)  NOT NULL,
    [Visible]       BIT           NOT NULL,
    [Description]   VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_ContactType] PRIMARY KEY CLUSTERED ([ContactTypeID] ASC),
    CONSTRAINT [IX_ContactType_CodeID] UNIQUE NONCLUSTERED ([CodeID] ASC)
);

