CREATE TABLE [dbo].[Application] (
    [ApplicationID]          BIGINT         IDENTITY (1, 1) NOT NULL,
    [DepartmentID]           BIGINT         NOT NULL,
    [ApplicationName]        VARCHAR (500)  NOT NULL,
    [ApplicationDescription] VARCHAR (1000) NOT NULL,
    [ApplicationCodeID]      BIGINT         NOT NULL,
    [ApplicationGroupOrder]  BIGINT         NOT NULL,
    [ApplicationVisible]     BIT            CONSTRAINT [DF_Application_ScreenVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_Application] PRIMARY KEY CLUSTERED ([ApplicationID] ASC),
    CONSTRAINT [FK_Application_Department] FOREIGN KEY ([DepartmentID]) REFERENCES [dbo].[Department] ([DepartmentID]),
    CONSTRAINT [IX_DepartmentApplicationCodeID] UNIQUE NONCLUSTERED ([DepartmentID] ASC, [ApplicationCodeID] ASC)
);

