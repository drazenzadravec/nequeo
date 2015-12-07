CREATE TABLE [dbo].[AccessType] (
    [AccessTypeID]      BIGINT        IDENTITY (1, 1) NOT NULL,
    [AccessName]        VARCHAR (50)  NOT NULL,
    [AccessDescription] VARCHAR (500) NOT NULL,
    [AccessCodeID]      BIGINT        NOT NULL,
    [AccessGroupOrder]  BIGINT        NOT NULL,
    [AccessVisible]     BIT           CONSTRAINT [DF_AccessType_AccessVisible] DEFAULT ((1)) NOT NULL,
    CONSTRAINT [PK_AccessType] PRIMARY KEY CLUSTERED ([AccessTypeID] ASC),
    CONSTRAINT [IX_AccessCodeID] UNIQUE NONCLUSTERED ([AccessCodeID] ASC)
);

