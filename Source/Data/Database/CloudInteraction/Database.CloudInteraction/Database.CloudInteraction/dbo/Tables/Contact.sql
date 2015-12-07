CREATE TABLE [dbo].[Contact] (
    [ContactID]     BIGINT IDENTITY (1, 1) NOT NULL,
    [UserID]        BIGINT NOT NULL,
    [UserContactID] BIGINT NOT NULL,
    [ContactTypeID] BIGINT NOT NULL,
    CONSTRAINT [PK_Contact] PRIMARY KEY CLUSTERED ([ContactID] ASC),
    CONSTRAINT [FK_Contact_ContactType] FOREIGN KEY ([ContactTypeID]) REFERENCES [dbo].[ContactType] ([ContactTypeID]),
    CONSTRAINT [FK_Contact_User] FOREIGN KEY ([UserID]) REFERENCES [dbo].[User] ([UserID]),
    CONSTRAINT [FK_Contact_User1] FOREIGN KEY ([UserContactID]) REFERENCES [dbo].[User] ([UserID])
);

