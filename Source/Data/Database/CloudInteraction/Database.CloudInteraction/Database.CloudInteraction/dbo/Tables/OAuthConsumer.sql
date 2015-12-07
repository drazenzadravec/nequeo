CREATE TABLE [dbo].[OAuthConsumer] (
    [OAuthConsumerID]        BIGINT         IDENTITY (1, 1) NOT NULL,
    [VerificationCodeFormat] BIGINT         NULL,
    [VerificationCodeLength] BIGINT         NULL,
    [VerificationCode]       NVARCHAR (500) NULL,
    [ClientID]               BIGINT         NOT NULL,
    [UserID]                 BIGINT         NULL,
    CONSTRAINT [PK_OAuthConsumer] PRIMARY KEY CLUSTERED ([OAuthConsumerID] ASC),
    CONSTRAINT [FK_OAuthConsumer_Client] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ClientID]),
    CONSTRAINT [FK_OAuthConsumer_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([UserID])
);

