
CREATE PROCEDURE [dbo].[GetActiveLoggedOnUserContacts]
	@UserID BIGINT, @LastActiveDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;

    SELECT t.* 
	FROM (SELECT * FROM [GetUniqueUserContacts] (@UserID)) AS t
	WHERE ([LastActivityDate] >= @LastActiveDate) AND ([LoggedIn] = 1)
END
RETURN