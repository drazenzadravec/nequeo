create PROCEDURE [dbo].[GetSelectedCustomerIncomeInvoiceDetailsBetweenDate]
	@FromDate datetime, @ToDate datetime, @CustomerID int
AS
	SELECT I.*, IDS.*, Cu.*
	FROM Invoices I, InvoiceDetails IDS, Customers Cu
	WHERE ( ((I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate)) AND
		 (Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IDS.InvoiceID))
RETURN