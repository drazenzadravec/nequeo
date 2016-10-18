CREATE TABLE [dbo].[Authentication_Client] (
    [AuthenticationID]  BIGINT         IDENTITY (1, 1) NOT NULL,
    [UniqueIdentifier]  NVARCHAR (100) NOT NULL,
    [Name]              NVARCHAR (500) NOT NULL,
    [EmailAddress]      NVARCHAR (200) NOT NULL,
    [Username]          NVARCHAR (100) NOT NULL,
    [Password]          NVARCHAR (50)  NOT NULL,
    [ApplicationName]   NVARCHAR (200) NOT NULL,
    [IsLoggedOn]        BIT            NOT NULL,
    [IsSuspended]       BIT            NOT NULL,
    [OnlineStatus]      NVARCHAR (50)  NOT NULL,
    [ActiveConnections] BIGINT         NOT NULL,
    [Value]             NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_Authentication_Client] PRIMARY KEY CLUSTERED ([AuthenticationID] ASC)
);

