CREATE TABLE [dbo].[DataMember] (
    [DataMemberID]      INT           IDENTITY (1, 1) NOT NULL,
    [Name]              VARCHAR (50)  NOT NULL,
    [Description]       VARCHAR (500) NOT NULL,
    [DataMemberTableID] INT           NOT NULL,
    CONSTRAINT [PK_DataMember] PRIMARY KEY CLUSTERED ([DataMemberID] ASC)
);

