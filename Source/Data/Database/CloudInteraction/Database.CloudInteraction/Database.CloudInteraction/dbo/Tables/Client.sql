CREATE TABLE [dbo].[Client] (
    [ClientID]                 BIGINT         IDENTITY (1, 1) NOT NULL,
    [ClientIdentifier]         NVARCHAR (100) NOT NULL,
    [ClientSecret]             NVARCHAR (100) NULL,
    [Callback]                 NVARCHAR (500) NULL,
    [FullName]                 NVARCHAR (500) NOT NULL,
    [ClientType]               BIGINT         NOT NULL,
    [OpenIDClaimedIdentifier]  NVARCHAR (150) NOT NULL,
    [OpenIDFriendlyIdentifier] NVARCHAR (150) NULL,
    CONSTRAINT [PK_Client] PRIMARY KEY CLUSTERED ([ClientID] ASC),
    CONSTRAINT [IX_ClientIdentifier] UNIQUE NONCLUSTERED ([ClientIdentifier] ASC)
);

