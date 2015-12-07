
CREATE PROCEDURE [dbo].[InsertChangeLog] 
	@UserID bigint, 
	@ScreenID bigint, 
	@ApplicationID bigint,
	@Change text, 
	@PrimaryKeyID varchar(200),
	@IPAddress varchar(500),
	@ChangeLogID bigint output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT INTO dbo.ChangeLog
			(UserID
			,ScreenID
			,ApplicationID
			,PrimaryKeyID
			,ModifiedDate
			,Change
			,IPAddress)
		VALUES
			(@UserID
			,@ScreenID
			,@ApplicationID
			,@PrimaryKeyID
			,GETDATE()
			,@Change
			,@IPAddress)
			
		SET @ChangeLogID = @@IDENTITY
		RETURN 0
	END TRY
	BEGIN CATCH
		-- Log the error that has occurred.
		DECLARE @ErrorNumber varchar(50), @ErrorMessage varchar(max)

		SET @ErrorNumber = CAST(ERROR_NUMBER() AS varchar(50))
		SET @ErrorMessage = CAST(ERROR_MESSAGE() AS varchar(max))
		
		EXECUTE [dbo].[InsertErrorLog]
			@ApplicationIdentifier = 1,
			@ProcessName = N'InsertChangeLog',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
