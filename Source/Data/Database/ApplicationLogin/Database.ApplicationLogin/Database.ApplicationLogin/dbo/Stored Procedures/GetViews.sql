
CREATE PROCEDURE [dbo].[GetViews] 
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
				'information_schema.views.Table_Schema AS [TableOwner],' + ' ' +
				'information_schema.views.TABLE_NAME AS [TableName]' + ' ' +
			'FROM information_schema.views' + ' ' +
			'ORDER BY' + ' ' +
				'information_schema.views.Table_Schema ASC'

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
			@ProcessName = N'GetViews',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
