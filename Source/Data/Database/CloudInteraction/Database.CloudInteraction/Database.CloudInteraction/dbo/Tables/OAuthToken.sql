CREATE TABLE [dbo].[OAuthToken] (
    [OAuthTokenID]    BIGINT          IDENTITY (1, 1) NOT NULL,
    [Token]           NVARCHAR (MAX)  NOT NULL,
    [TokenSecret]     NVARCHAR (MAX)  NOT NULL,
    [State]           INT             NOT NULL,
    [IssueDateUtc]    DATETIME        NOT NULL,
    [ExpiryDateUtc]   DATETIME        NOT NULL,
    [OAuthConsumerID] BIGINT          NOT NULL,
    [Scope]           NVARCHAR (500)  NULL,
    [TokenVerifier]   NVARCHAR (MAX)  NULL,
    [TokenCallback]   NVARCHAR (4000) NULL,
    [ConsumerVersion] NVARCHAR (4000) NULL,
    [Context]         NVARCHAR (200)  NOT NULL,
    [TokenType]       NVARCHAR (50)   NOT NULL,
    CONSTRAINT [PK_OAuthToken] PRIMARY KEY CLUSTERED ([OAuthTokenID] ASC),
    CONSTRAINT [FK_OAuthToken_OAuthConsumer] FOREIGN KEY ([OAuthConsumerID]) REFERENCES [dbo].[OAuthConsumer] ([OAuthConsumerID])
);

