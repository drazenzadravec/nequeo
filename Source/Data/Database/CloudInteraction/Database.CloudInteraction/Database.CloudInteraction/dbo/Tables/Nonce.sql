CREATE TABLE [dbo].[Nonce] (
    [Context]         NVARCHAR (200) NOT NULL,
    [Code]            NVARCHAR (200) NOT NULL,
    [Timestamp]       DATETIME       NOT NULL,
    [OAuthConsumerID] BIGINT         NOT NULL,
    CONSTRAINT [PK_Nonce] PRIMARY KEY CLUSTERED ([Context] ASC, [Code] ASC, [Timestamp] ASC),
    CONSTRAINT [FK_Nonce_OAuthConsumer] FOREIGN KEY ([OAuthConsumerID]) REFERENCES [dbo].[OAuthConsumer] ([OAuthConsumerID])
);

