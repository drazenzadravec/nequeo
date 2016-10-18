CREATE TABLE [dbo].[GenericData] (
    [GenericDataID] INT           IDENTITY (1, 1) NOT NULL,
    [DataValue]     VARCHAR (MAX) NOT NULL,
    [DataValueType] VARCHAR (500) NOT NULL,
    [Description]   VARCHAR (500) NOT NULL,
    CONSTRAINT [PK_GenericData] PRIMARY KEY CLUSTERED ([GenericDataID] ASC)
);

