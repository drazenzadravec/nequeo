CREATE TABLE [dbo].[AccountTransactions] (
    [AccountTransactionID] INT           IDENTITY (1, 1) NOT NULL,
    [AccountID]            INT           NOT NULL,
    [DataMember]           INT           NULL,
    [PaymentDate]          DATETIME      NOT NULL,
    [TransactionType]      VARCHAR (50)  NOT NULL,
    [Amount]               MONEY         NOT NULL,
    [Description]          VARCHAR (100) NULL,
    [ReferenceNumber]      VARCHAR (50)  NULL,
    [PaidToFrom]           VARCHAR (50)  NULL,
    [PaidToFromID]         INT           NULL,
    [PaidToFromRefID]      INT           NULL,
    [Comments]             VARCHAR (300) NULL,
    CONSTRAINT [PK_AccountTransactions] PRIMARY KEY CLUSTERED ([AccountTransactionID] ASC)
);

