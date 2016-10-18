CREATE TABLE [dbo].[Token_Client] (
    [TokenID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [UniqueIdentifier] NVARCHAR (100) NOT NULL,
    [ServiceName]      NVARCHAR (100) NOT NULL,
    [Token]            NVARCHAR (50)  NOT NULL,
    [DateAdded]        DATETIME       NOT NULL,
    [Permission]       NVARCHAR (MAX) NOT NULL,
    [Value]            NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Token] PRIMARY KEY CLUSTERED ([TokenID] ASC)
);

