
CREATE PROCEDURE [dbo].[GetDatabaseReferenceKeys]
	@DataBase varchar(max),
	@TableName varchar(max)
AS
BEGIN
	--- SET NOCOUNT ON added to prevent extra result sets from
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
			'SELECT' + ' ' +
				'CAST(sysobjects.Name as varchar(max)) as [TableName],' + ' ' +
				'CAST(syscolumns.Name as varchar(max)) as [ColumnName],' + ' ' +
				'CAST(syscolumns.isNullable as bit) as [IsNullable],' + ' ' +
				'CAST(syscolumns.Length as bigint) as [Length],' + ' ' +
				'CAST(systypes.Name as varchar(max)) as [ColumnType],' + ' ' +
				'CAST(OBJECT_NAME(sysforeignkeys.rkeyid) as varchar(max)) as [ForeignKeyTable]' + ' ' +
			'FROM' + ' ' +
				'syscolumns, sysobjects, systypes, sysforeignkeys' + ' ' +
			'WHERE syscolumns.ID = sysobjects.ID' + ' ' +
				'and syscolumns.xusertype = systypes.xusertype' + ' ' +
				'and syscolumns.id = sysforeignkeys.fkeyid' + ' ' +
				'and syscolumns.colid = sysforeignkeys.fkey' + ' ' +
				'and sysobjects.status <> 1' + ' ' +
				'and sysobjects.type = ''U''' + ' ' +
				'and systypes.name <> ''XML''' + ' ' +
				'and OBJECT_NAME(sysforeignkeys.rkeyid) =''' + @TableNameValue + ''''

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
			@ProcessName = N'GetDatabaseReferenceKeys',
			@ErrorCode = @ErrorNumber,
			@ErrorDescription = @ErrorMessage

		-- Return (-1) indicates an error.
        RETURN -1;
    END CATCH
END
