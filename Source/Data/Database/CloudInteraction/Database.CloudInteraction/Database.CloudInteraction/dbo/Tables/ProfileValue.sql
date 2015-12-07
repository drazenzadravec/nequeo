CREATE TABLE [dbo].[ProfileValue] (
    [ProfileValueID] BIGINT         IDENTITY (1, 1) NOT NULL,
    [ProfileID]      BIGINT         NOT NULL,
    [PropertyName]   VARCHAR (200)  NOT NULL,
    [PropertyValue]  NVARCHAR (MAX) NOT NULL,
    [PropertyType]   VARCHAR (200)  CONSTRAINT [DF_ProfileValue_PropertyType] DEFAULT ('System.String') NOT NULL,
    CONSTRAINT [PK_ProfileValue] PRIMARY KEY CLUSTERED ([ProfileValueID] ASC),
    CONSTRAINT [FK_ProfileValue_Profile] FOREIGN KEY ([ProfileID]) REFERENCES [dbo].[Profile] ([ProfileID])
);

