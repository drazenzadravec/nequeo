
CREATE PROCEDURE [dbo].[GetTableColumns] 
	@DataBase varchar(max),
	@TableName varchar(max),
	@Owner varchar(max)
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

			'SELECT DISTINCT' + ' ' +
				'dbo.syscolumns.name AS ColumnName,' + ' ' +
				'(SELECT TOP 1 dbo.systypes.name' + ' ' +
				'FROM dbo.systypes' + ' ' +
				'WHERE dbo.systypes.xtype = dbo.syscolumns.xtype) AS ColumnType,' + ' ' +
				'dbo.syscolumns.isnullable AS ColumnNullable,' + ' ' +
				'dbo.syscolumns.colorder AS ColumnOrder,' + ' ' +
				'dbo.syscolumns.iscomputed AS IsComputed,' + ' ' +
				'dbo.syscolumns.length AS Length,' + ' ' +
				'dbo.syscolumns.colstat AS PrimaryKeySeed INTO #t_obj' + ' ' +
			'FROM' + ' ' +
				'dbo.syscolumns INNER JOIN' + ' ' +
					'dbo.sysobjects ON dbo.syscolumns.id = dbo.sysobjects.id INNER JOIN' + ' ' +
						'dbo.systypes ON dbo.syscolumns.xtype = dbo.systypes.xtype' + ' ' +
			'WHERE' + ' ' +
				'(dbo.sysobjects.name = ''' + @TableNameValue + ''')' + ' ' +
			'ORDER BY' + ' ' +
				'dbo.syscolumns.colorder ASC' + ' ' +

			'SELECT' + ' ' +
				'CAST(ColumnName AS varchar(max)) AS [ColumnName],' + ' ' +
				'CAST(ColumnType AS varchar(max)) AS [ColumnType],' + ' ' +
				'CAST(ColumnNullable AS Bit) AS [ColumnNullable],' + ' ' +
				'CAST(ColumnOrder AS Int) AS [ColumnOrder],' + ' ' +
				'CAST(IsComputed AS Bit) AS [IsComputed],' + ' ' +
				'CAST(Length AS BigInt) AS [Length],' + ' ' +
				'CAST(PrimaryKeySeed AS Bit) AS [PrimaryKeySeed]' + ' ' +
			'FROM #t_obj' + ' ' +
			'DROP TABLE #t_obj'

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
			@ProcessName = N'GetTableColumns',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
