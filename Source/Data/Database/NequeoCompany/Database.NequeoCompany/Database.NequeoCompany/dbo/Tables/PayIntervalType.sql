CREATE TABLE [dbo].[PayIntervalType] (
    [PayIntervalTypeID] INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (50)  NOT NULL,
    [Description]       VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_PayIntervalType] PRIMARY KEY CLUSTERED ([PayIntervalTypeID] ASC)
);

