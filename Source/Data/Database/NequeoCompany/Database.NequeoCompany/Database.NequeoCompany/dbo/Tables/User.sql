CREATE TABLE [dbo].[User] (
    [UserID]        INT           IDENTITY (1, 1) NOT NULL,
    [FirstName]     VARCHAR (500) NOT NULL,
    [LastName]      VARCHAR (MAX) NOT NULL,
    [Suspended]     BIT           NOT NULL,
    [LoginUserName] VARCHAR (50)  NOT NULL,
    [LoginPassword] VARCHAR (500) NOT NULL,
    [UserType] VARCHAR(MAX) NOT NULL, 
    CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED ([UserID] ASC)
);

