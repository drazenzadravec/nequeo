
CREATE PROCEDURE [dbo].[GetDatabaseCurrentTableValues]
	@DataBase varchar(max),
	@TableName varchar(max), 
	@PrimaryKeyName varchar(max),
	@PrimaryKeyID varchar(max)
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    DECLARE @PrimaryKeyIDValue varchar(max)
	DECLARE @PrimaryKeyNameValue varchar(max)
	DECLARE @TableNameValue varchar(max)
	DECLARE @DataBaseValue varchar(max)
	DECLARE @SQL varchar(max)

	SET @DataBaseValue = @DataBase
	SET @PrimaryKeyIDValue = @PrimaryKeyID
	SET @PrimaryKeyNameValue = @PrimaryKeyName
	SET @TableNameValue = @TableName

	BEGIN TRY
		SET @SQL = 
			'USE [' + @DataBaseValue + ']' + ' ' +

			'SELECT' + ' ' +
				'DISTINCT dbo.syscolumns.name AS ColName' + ' ' +
				'INTO #t_obj' + ' ' +
			'FROM' + ' ' +  
				'dbo.syscolumns INNER JOIN' + ' ' +
					'dbo.sysobjects ON dbo.syscolumns.id = dbo.sysobjects.id INNER JOIN' + ' ' +
						'dbo.systypes ON dbo.syscolumns.xtype = dbo.systypes.xtype' + ' ' +
			'WHERE' + ' ' +
				'(dbo.sysobjects.name = ''' + @TableNameValue + ''') AND' + ' ' +
				'(dbo.systypes.status <> 1) AND' + ' ' +
				'(dbo.systypes.name <> ''XML'')' + ' ' +
			'ORDER BY' + ' ' +
				'dbo.syscolumns.name DESC' + ' ' +

			'DECLARE @ColumnName varchar(max)' + ' ' +
			'DECLARE @FinalConc varchar(max)' + ' ' +
			'DECLARE @SQLInternal varchar(max)' + ' ' +

			'SET @ColumnName = '' ''' + ' ' +

			'DECLARE @col_name varchar(max)' + ' ' +
			'DECLARE cur CURSOR FOR' + ' ' +

			'SELECT * FROM #t_obj' + ' ' +
			'OPEN cur' + ' ' +

			'FETCH NEXT FROM cur' + ' ' +
			'INTO @col_name' + ' ' +
			'WHILE @@FETCH_STATUS = 0' + ' ' +
			'BEGIN' + ' ' +
				'SET @ColumnName = ''''''|'' + @col_name + '':'''''' + '' + CAST(ISNULL(['' + @col_name + ''], 0) AS VARCHAR(max))'' + '' + '' + @ColumnName' + ' ' +
				
				'FETCH NEXT FROM cur' + ' ' +
				'INTO @col_name' + ' ' +
			'END' + ' ' +
			'CLOSE cur' + ' ' +
			'DEALLOCATE cur' + ' ' +

			'DROP TABLE #t_obj' + ' ' +

			'SET @FinalConc = SUBSTRING(@ColumnName, 1, LEN(@ColumnName) - 1)' + ' ' +

			'SET @SQLInternal =
				''SELECT ('' + @FinalConc + '') AS [ChangeValue]'' + '' '' +
				''FROM ' + @TableNameValue + ''' + '' '' + 
				''WHERE ' + @PrimaryKeyNameValue + '' + ' = ' + STR(@PrimaryKeyIDValue) + ''' ' +

				'EXEC (@SQLInternal)'

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
			@ProcessName = N'GetDatabaseCurrentTableValues',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
