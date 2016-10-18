CREATE TABLE [dbo].[State] (
    [StateID]   INT           IDENTITY (1, 1) NOT NULL,
    [ShortName] VARCHAR (50)  NOT NULL,
    [LongName]  VARCHAR (100) NOT NULL,
    CONSTRAINT [PK_State] PRIMARY KEY CLUSTERED ([StateID] ASC)
);

