
CREATE PROCEDURE [dbo].[UpdateLoginHistory] 
	@LoginHistoryID bigint
AS
BEGIN
	DECLARE @RowCount int
	SET @RowCount = 0
	
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		BEGIN TRANSACTION
		
		UPDATE dbo.LoginHistory
		SET LogoutDate = GETDATE()
		WHERE LoginHistoryID = @LoginHistoryID
		
		SET @RowCount = @@ROWCOUNT
		COMMIT TRANSACTION

		IF @RowCount = 0 
            BEGIN 
				-- Return (-2) indicates a concurrency.
                RETURN -2;
            END 
		ELSE
			BEGIN 
                RETURN 0;
            END 
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION
		
		-- Log the error that has occurred.
		DECLARE @ErrorNumber varchar(50), @ErrorMessage varchar(max)

		SET @ErrorNumber = CAST(ERROR_NUMBER() AS varchar(50))
		SET @ErrorMessage = CAST(ERROR_MESSAGE() AS varchar(max))
		
		EXECUTE [dbo].[InsertErrorLog]
			@ApplicationIdentifier = 1,
			@ProcessName = N'UpdateLoginHistory',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
