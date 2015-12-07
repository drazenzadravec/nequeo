
CREATE PROCEDURE [dbo].[GetUserScreenAccess] 
	@UserID bigint,
	@ApplicationID bigint
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	SET NOCOUNT ON;

	BEGIN TRY
		SELECT dbo.ApplicationUser.Suspended,

			(SELECT ScreenCodeID 
			FROM dbo.Screen
			WHERE (dbo.Screen.ScreenID = dbo.ScreenAccess.ScreenID)) AS [ScreenCode],

			(SELECT AccessCodeID
			FROM dbo.AccessType
			WHERE (dbo.AccessType.AccessTypeID = dbo.ScreenAccess.AccessTypeID)) AS [AccessCode]

		FROM dbo.ApplicationUser INNER JOIN dbo.ScreenAccess
			ON dbo.ApplicationUser.RoleTypeID = dbo.ScreenAccess.RoleTypeID
		WHERE (dbo.ApplicationUser.ApplicationID = @ApplicationID) AND
			(dbo.ApplicationUser.UserID = @UserID)
		ORDER BY dbo.ScreenAccess.ScreenID ASC, 
			dbo.ScreenAccess.AccessTypeID ASC

		RETURN 0
	END TRY
	BEGIN CATCH
		-- Log the error that has occurred.
		DECLARE @ErrorNumber varchar(50), @ErrorMessage varchar(max)

		SET @ErrorNumber = CAST(ERROR_NUMBER() AS varchar(50))
		SET @ErrorMessage = CAST(ERROR_MESSAGE() AS varchar(max))
		
		EXECUTE [dbo].[InsertErrorLog]
			@ApplicationIdentifier = @ApplicationID,
			@ProcessName = N'GetUserScreenAccess',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
