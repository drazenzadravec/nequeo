create PROCEDURE [dbo].[GetSelectedCustomerIncomeInvoiceProductsBetweenDate]
	@FromDate datetime, @ToDate datetime, @CustomerID int
AS
	SELECT I.*, IPS.*, Cu.*, P.*
	FROM Invoices I, InvoiceProducts IPS, Customers Cu, Products P
	WHERE ( ((I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate)) AND
		(Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IPS.InvoiceID) AND (IPS.ProductID = P.ProductID))
RETURN