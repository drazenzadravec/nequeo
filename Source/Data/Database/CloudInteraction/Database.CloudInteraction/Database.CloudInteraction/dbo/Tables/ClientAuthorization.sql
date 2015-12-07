CREATE TABLE [dbo].[ClientAuthorization] (
    [ClientAuthorizationID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [CreatedOnUtc]          DATETIME       NOT NULL,
    [ClientID]              BIGINT         NOT NULL,
    [UserID]                BIGINT         NULL,
    [Scope]                 NVARCHAR (MAX) NULL,
    [ExpirationDateUtc]     DATETIME       NULL,
    [NonceCode]             NVARCHAR (200) NOT NULL,
    [CodeKey]               NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_ClientAuthorization] PRIMARY KEY CLUSTERED ([ClientAuthorizationID] ASC),
    CONSTRAINT [FK_ClientAuthorization_Client] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ClientID]),
    CONSTRAINT [FK_ClientAuthorization_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([UserID])
);

