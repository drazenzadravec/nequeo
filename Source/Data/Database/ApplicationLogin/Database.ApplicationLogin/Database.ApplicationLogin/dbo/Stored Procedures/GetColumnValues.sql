
CREATE PROCEDURE [dbo].[GetColumnValues]
	@DataBase varchar(max),
	@TableName varchar(max),
	@ColumnName varchar(max),
	@ColumnIndicatorName varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @TableNameValue varchar(max)
	DECLARE @DataBaseValue varchar(max)
	DECLARE @ColumnNameValue varchar(max)
	DECLARE @ColumnIndicatorNameValue varchar(max)
	DECLARE @SQL varchar(max)

	SET @DataBaseValue = @DataBase
	SET @TableNameValue = @TableName
	SET @ColumnNameValue = @ColumnName
	SET @ColumnIndicatorNameValue = @ColumnIndicatorName

	BEGIN TRY
		SET @SQL = 
			'USE [' + @DataBaseValue + ']' + ' ' +

			'SELECT CAST(' + @ColumnNameValue + ' AS varchar(max)) AS [ColumnValue],' + ' ' +
				@ColumnIndicatorNameValue + ' AS [ColumnIndicator]' + ' ' +
			'FROM ' + @TableNameValue

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
			@ProcessName = N'GetColumnValues',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
