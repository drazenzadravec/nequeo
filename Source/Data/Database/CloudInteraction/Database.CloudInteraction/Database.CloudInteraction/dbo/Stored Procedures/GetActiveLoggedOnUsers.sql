
CREATE PROCEDURE [dbo].[GetActiveLoggedOnUsers] 
	@UserID BIGINT, @LastActiveDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;

    SELECT * 
	FROM [User]
	WHERE ([LastActivityDate] >= @LastActiveDate) AND ([LoggedIn] = 1)
END
RETURN