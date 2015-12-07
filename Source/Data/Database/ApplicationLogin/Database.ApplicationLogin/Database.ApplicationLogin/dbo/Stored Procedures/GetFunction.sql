
CREATE PROCEDURE GetFunction
	@DataBase varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @DataBaseValue varchar(max)
	DECLARE @SQL varchar(max)

	SET @DataBaseValue = @DataBase

	BEGIN TRY
		SET @SQL = 
			'USE [' + @DataBaseValue + ']' + ' ' +

			'SELECT' + ' ' +
				'information_schema.ROUTINES.ROUTINE_NAME  AS [FunctionName],' + ' ' +
				'information_schema.ROUTINES.ROUTINE_SCHEMA  AS [FunctionOwner]' + ' ' +  
			'FROM  information_schema.ROUTINES' + ' ' +
			'WHERE' + ' ' +
				'information_schema.ROUTINES.ROUTINE_TYPE = ''FUNCTION''' + ' ' +
			'ORDER BY' + ' ' +
				'information_schema.ROUTINES.ROUTINE_NAME ASC'

		EXEC (@SQL)
		RETURN 0
	END TRY
	BEGIN CATCH
		-- Log the error that has occurred.
		DECLARE @ErrorNumber varchar(50), @ErrorMessage varchar(max)

		SET @ErrorNumber = CAST(ERROR_NUMBER() AS varchar(50))
		SET @ErrorMessage = CAST(ERROR_MESSAGE() AS varchar(max))
		
		EXECUTE [dbo].[InsertErrorLog]
			@ApplicationIdentifier = 1,
			@ProcessName = N'GetFunction',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
