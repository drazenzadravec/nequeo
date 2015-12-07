CREATE TABLE [dbo].[User] (
    [UserID]         BIGINT         IDENTITY (1, 1) NOT NULL,
    [LoginUsername]  VARCHAR (50)   NOT NULL,
    [LoginPassword]  VARCHAR (500)  NOT NULL,
    [Comments]       VARCHAR (1000) NULL,
    [UserSuspended]  BIT            CONSTRAINT [DF_User_UserSuspended] DEFAULT ((0)) NOT NULL,
    [ModifiedDate]   DATETIME       NULL,
    [RowVersionData] ROWVERSION     NULL,
    [UserAddressID]  BIGINT         NOT NULL,
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserID] ASC),
    CONSTRAINT [FK_User_UserAddress] FOREIGN KEY ([UserAddressID]) REFERENCES [dbo].[UserAddress] ([UserAddressID]),
    CONSTRAINT [IX_LoginUsername] UNIQUE NONCLUSTERED ([LoginUsername] ASC)
);

