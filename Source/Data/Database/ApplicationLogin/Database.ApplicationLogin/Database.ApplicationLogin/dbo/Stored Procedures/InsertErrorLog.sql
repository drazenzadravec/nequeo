
CREATE PROCEDURE [dbo].[InsertErrorLog]
	@ApplicationIdentifier bigint,
	@ProcessName varchar(200), 
	@ErrorCode varchar(50),
	@ErrorDescription varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	BEGIN TRY
		INSERT INTO dbo.ErrorLog
			(ApplicationID
			,ProcessName
			,ErrorCode
			,ErrorDescription
			,ErrorDate)
		VALUES
			(@ApplicationIdentifier
			,@ProcessName
			,@ErrorCode
			,@ErrorDescription
			,GETDATE())
		
		RETURN 0
	END TRY
	BEGIN CATCH
		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
