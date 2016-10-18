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

INSERT INTO [dbo].[AccountType]([Name],[Description])VALUES('Freedom Business','Freedom Business')
INSERT INTO [dbo].[AccountType]([Name],[Description])VALUES('Cheque','Cheque')
INSERT INTO [dbo].[AccountType]([Name],[Description])VALUES('Credit','Credit')
INSERT INTO [dbo].[AccountType]([Name],[Description])VALUES('Keycard','Keycard')
INSERT INTO [dbo].[AccountType]([Name],[Description])VALUES('Passbook','Passbook')
INSERT INTO [dbo].[AccountType]([Name],[Description])VALUES('Loan','Loan')
INSERT INTO [dbo].[AccountType]([Name],[Description])VALUES('Other','Other')
GO

INSERT INTO [dbo].[AssetCategory]([Category],[Comments])VALUES('Software','')
INSERT INTO [dbo].[AssetCategory]([Category],[Comments])VALUES('Hardware','')
INSERT INTO [dbo].[AssetCategory]([Category],[Comments])VALUES('Furniture','')
INSERT INTO [dbo].[AssetCategory]([Category],[Comments])VALUES('Stationary','')
INSERT INTO [dbo].[AssetCategory]([Category],[Comments])VALUES('Automotive','')
INSERT INTO [dbo].[AssetCategory]([Category],[Comments])VALUES('Other','')
GO

INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Invoice Details','Invoice Details',10)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Invoice Products','Invoice Products',16)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Employee Wages','Employee Wages',8)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Employee Super','Employee Super',9)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Employee PAYG','Employee PAYG',15)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Vendor Details','Vendor Details',14)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Business Activity Statement','Business Activity Statement',17)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Company Tax Return','Company Tax Return',18)
INSERT INTO [dbo].[DataMember]([Name],[Description],[DataMemberTableID])VALUES('Other','Other',-1)
GO

INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(0,'Employees','Employees','0','an Employee','EmployeeID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(1,'Vendors','Vendors','1','a Vendor','VendorID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(2,'Customers','Customers','2','a Customer','CustomerID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(3,'Invoices','Invoices','3','an Invoice','InvoiceID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(4,'Accounts','Accounts','4','an Account','AccountID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(5,'Assets','Assets','5','an Asset','AssetID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(6,'Products','Products','6','a Product','ProductID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(7,'Companies','Company','7','a Company','CompanyID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(8,'Wages','Employee Wages','0','an Employee Wage','WageID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(9,'Super','Employee Super','0','an Employee Superannuation','SuperID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(10,'InvoiceDetails','Invoice Details','3','an Invoice Detail','InvoiceDetailsID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(11,'EmployeeBankAccounts','Employee Accounts','0','an Employee Bank Account','AccountID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(12,'EmployeeSuperAccounts','Employee Super Funds','0','an Employee Super Fund','SuperFundID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(13,'AccountTransactions','Account Transactions','4','an Account Transaction','AccountTransactionID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(14,'VendorDetails','Vendor Details','1','a Vendor Detail','VendorDetailsID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(15,'EmployeePAYG','Employee PAYG','0','an Employee PAYG','PAYGID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(16,'InvoiceProducts','Invoice Products','3','an Invoice Product','InvoiceProductID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(17,'BAS','Business Activity Statement','7','a Business Activity Statement','BASID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(18,'TaxReturn','Company Tax Return','7','a Company Tax Return','TaxReturnID')
INSERT INTO [dbo].[DataMemberTables]([DataMemberID],[DataTables],[TableName],[Reference],[NameTo],[DataTableKeyName])VALUES(19,'CompanyPAYGInstalment','Company PAYG Instalment','7','a Company PAYG Instalment','PAYGInstID')
GO

INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Cost of Sales','Cost of Sales')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Contractor, Sub-Contractor and Commission Expenses','Contractor, Sub-Contractor and Commission Expenses')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Rent Expenses','Rent Expenses')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Motor Vehicle Expenses','Motor Vehicle Expenses')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Repairs and Maintenace','Repairs and Maintenace')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('All Other Expenses','All Other Expenses')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Lease Expenses within Australia','Lease Expenses within Australia')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Interest Expenses within Australia','Interest Expenses within Australia')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Royalty Expenses within Australia','Royalty Expenses within Australia')
INSERT INTO [dbo].[ExpenseType]([Description],[Name])VALUES('Depreciation Expenses','Depreciation Expenses')
GO

INSERT INTO [dbo].[GenericData]([DataValue],[DataValueType],[Description])VALUES('10.0','System.Decimal','Australian GST Rate')
GO

INSERT INTO [dbo].[GstIncomeType]([Name])VALUES('Included')
INSERT INTO [dbo].[GstIncomeType]([Name])VALUES('Calculated')
GO

INSERT INTO [dbo].[IncomeType]([Description],[Name])VALUES('Other Sales of Goods and Services','Other Sales of Goods and Services')
INSERT INTO [dbo].[IncomeType]([Description],[Name])VALUES('Other Income','Other Income')
INSERT INTO [dbo].[IncomeType]([Description],[Name])VALUES('Interest','Interest')
INSERT INTO [dbo].[IncomeType]([Description],[Name])VALUES('Payments Where ABN not Quoted','Payments Where ABN not Quoted')
INSERT INTO [dbo].[IncomeType]([Description],[Name])VALUES('Rent and Other Leasing and Hiring Income','Rent and Other Leasing and Hiring Income')
INSERT INTO [dbo].[IncomeType]([Description],[Name])VALUES('Dividends','Dividends')
INSERT INTO [dbo].[IncomeType]([Description],[Name])VALUES('Assessable Government Industry Payments','Assessable Government Industry Payments')
GO

INSERT INTO [dbo].[PayIntervalType]([Name],[Description])VALUES('Weekly','Weekly pay interval type')
INSERT INTO [dbo].[PayIntervalType]([Name],[Description])VALUES('Fortnightly','Fortnightly pay interval')
INSERT INTO [dbo].[PayIntervalType]([Name],[Description])VALUES('Monthly','Monthly pay interval')
GO

INSERT INTO [dbo].[PaymentType]([Name],[Description])VALUES('Refund','Refund')
INSERT INTO [dbo].[PaymentType]([Name],[Description])VALUES('Payment','Payment')
GO

INSERT INTO [dbo].[ProductCategory]([Category],[Comments])VALUES('Software','')
INSERT INTO [dbo].[ProductCategory]([Category],[Comments])VALUES('Hardware','')
INSERT INTO [dbo].[ProductCategory]([Category],[Comments])VALUES('Furniture','')
INSERT INTO [dbo].[ProductCategory]([Category],[Comments])VALUES('Stationary','')
INSERT INTO [dbo].[ProductCategory]([Category],[Comments])VALUES('Automotive','')
INSERT INTO [dbo].[ProductCategory]([Category],[Comments])VALUES('Other','')
GO

INSERT INTO [dbo].[ProductStatus]([StatusName],[Description])VALUES('New','')
INSERT INTO [dbo].[ProductStatus]([StatusName],[Description])VALUES('Upgrade Pending','')
INSERT INTO [dbo].[ProductStatus]([StatusName],[Description])VALUES('Disposal Pending','')
INSERT INTO [dbo].[ProductStatus]([StatusName],[Description])VALUES('Other','')
GO

INSERT INTO [dbo].[ProductSubCategory]([CategoryID],[SubCategoryName],[Comments])VALUES(1,'Database','')
INSERT INTO [dbo].[ProductSubCategory]([CategoryID],[SubCategoryName],[Comments])VALUES(1,'Programming','')
INSERT INTO [dbo].[ProductSubCategory]([CategoryID],[SubCategoryName],[Comments])VALUES(1,'Development','')
INSERT INTO [dbo].[ProductSubCategory]([CategoryID],[SubCategoryName],[Comments])VALUES(1,'Spread Sheet','')
INSERT INTO [dbo].[ProductSubCategory]([CategoryID],[SubCategoryName],[Comments])VALUES(1,'Word Processing','')
GO

INSERT INTO [dbo].[PurchaseType]([Name],[Description])VALUES('Non-Capital','All non capital purchases')
INSERT INTO [dbo].[PurchaseType]([Name],[Description])VALUES('Capital','All capital purchases')
GO

INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('NSW','New South Wales')
INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('QLD','Queensland')
INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('ACT','Australian Capital Territory')
INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('VIC','Victoria')
INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('TAS','Tasmainia')
INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('SA','South Australia')
INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('WA','Western Australia')
INSERT INTO [dbo].[State]([ShortName],[LongName])VALUES('NT','Northern Territory')
GO

INSERT INTO [dbo].[TransactionType]([Name],[Description])VALUES('Credit','Credit')
INSERT INTO [dbo].[TransactionType]([Name],[Description])VALUES('Debit','Debit')
GO

INSERT INTO [dbo].[UserType]([Name],[Description])VALUES('Administrator','Administrator')
INSERT INTO [dbo].[UserType]([Name],[Description])VALUES('Finance','Finance')
INSERT INTO [dbo].[UserType]([Name],[Description])VALUES('Sales','Sales')
INSERT INTO [dbo].[UserType]([Name],[Description])VALUES('Purchase','Purchase')
INSERT INTO [dbo].[UserType]([Name],[Description])VALUES('User','User')
GO

INSERT INTO [dbo].[User]([FirstName],[LastName],[Suspended],[LoginUserName],[LoginPassword],[UserType])VALUES('Admin','Super',0,'admin','3683C2ECEACA775D93246FFE0764FDC0','Administrator')
GO