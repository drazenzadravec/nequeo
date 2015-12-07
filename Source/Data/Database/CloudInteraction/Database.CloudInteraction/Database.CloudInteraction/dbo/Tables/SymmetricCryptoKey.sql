CREATE TABLE [dbo].[SymmetricCryptoKey] (
    [Bucket]          NVARCHAR (200)   NOT NULL,
    [Handle]          NVARCHAR (200)   NOT NULL,
    [ExpiresUtc]      DATETIME         NOT NULL,
    [Secret]          VARBINARY (8000) NOT NULL,
    [ClientID]        BIGINT           NOT NULL,
    [CertificatePath] NVARCHAR (MAX)   NULL,
    CONSTRAINT [PK_SymmetricCryptoKey] PRIMARY KEY CLUSTERED ([Bucket] ASC, [Handle] ASC),
    CONSTRAINT [FK_SymmetricCryptoKey_Client] FOREIGN KEY ([ClientID]) REFERENCES [dbo].[Client] ([ClientID])
);

