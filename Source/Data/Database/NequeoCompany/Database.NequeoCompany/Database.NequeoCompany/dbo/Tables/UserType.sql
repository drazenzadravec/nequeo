﻿CREATE TABLE [dbo].[UserType]
(
	[UserTypeID] BIGINT NOT NULL PRIMARY KEY IDENTITY, 
    [Name] VARCHAR(50) NOT NULL, 
    [Description] VARCHAR(MAX) NULL
)