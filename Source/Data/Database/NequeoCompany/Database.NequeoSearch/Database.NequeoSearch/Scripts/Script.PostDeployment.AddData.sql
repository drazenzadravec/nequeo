/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/

INSERT INTO [dbo].[DBTable]([TableName])VALUES('Accounts')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('AccountTransactions')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Assets')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('BAS')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Companies')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('CompanyPAYGInstalment')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Customers')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('EmployeeBankAccounts')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('EmployeePAYG')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Employees')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('EmployeeSuperAccounts')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('InvoiceDetails')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('InvoiceProducts')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Invoices')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Products')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Super')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('TaxReturn')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('VendorDetails')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Vendors')
INSERT INTO [dbo].[DBTable]([TableName])VALUES('Wages')
GO