
CREATE PROCEDURE [dbo].[GetDatabasePrimaryKeys] 
	@DataBase varchar(max),
	@TableName varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @TableNameValue varchar(max)
	DECLARE @DataBaseValue varchar(max)
	DECLARE @SQL varchar(max)

	SET @DataBaseValue = @DataBase
	SET @TableNameValue = @TableName

	BEGIN TRY
		SET @SQL = 
			'USE [' + @DataBaseValue + ']' + ' ' +
			
            'SELECT c.COLUMN_NAME AS PrimaryKeyName' + ' ' +
            'FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS pk,' + ' ' +
                'INFORMATION_SCHEMA.KEY_COLUMN_USAGE c' + ' ' +
            'WHERE pk.TABLE_NAME = ''' + @TableName + '''' + ' ' +
                'and CONSTRAINT_TYPE = ''PRIMARY KEY''' + ' ' +
                'and c.TABLE_NAME = pk.TABLE_NAME' + ' ' +
                'and c.CONSTRAINT_NAME = pk.CONSTRAINT_NAME'

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
			@ProcessName = N'GetDatabasePrimaryKeys',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
