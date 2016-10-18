CREATE TABLE [dbo].[DataMemberTables] (
    [DataMemberID]     INT          NOT NULL,
    [DataTables]       VARCHAR (50) NOT NULL,
    [TableName]        VARCHAR (50) NOT NULL,
    [Reference]        VARCHAR (50) NOT NULL,
    [NameTo]           VARCHAR (50) NULL,
    [DataTableKeyName] VARCHAR (50) NULL
);

