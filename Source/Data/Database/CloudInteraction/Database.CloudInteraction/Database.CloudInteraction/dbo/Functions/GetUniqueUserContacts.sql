
CREATE FUNCTION [dbo].[GetUniqueUserContacts] 
(	
	@UserID BIGINT
)
RETURNS TABLE 
AS
RETURN 
(
	(SELECT [User].*, t.[CodeID], t.[Name]
	FROM (SELECT * FROM [dbo].[GetUserContacts] (@UserID)) AS t INNER JOIN [User] ON
		t.[UserContactID] = [User].[UserID]
	WHERE (t.[UserContactID] != @UserID))
	UNION
	(SELECT [User].*, t.[CodeID], t.[Name]
	FROM (SELECT * FROM [dbo].[GetUserContacts] (@UserID)) AS t INNER JOIN [User] ON
		t.[CUserID] = [User].[UserID]
	WHERE (t.[CUserID] != @UserID))
)
