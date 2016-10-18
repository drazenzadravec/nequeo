CREATE PROCEDURE GetSelectedCustomerInvoiceInvoiceProductsBetweenDate
	@FromDate datetime, @ToDate datetime, @CustomerID int
AS
	SELECT I.*, IPS.*, Cu.*, P.*
	FROM Invoices I, InvoiceProducts IPS, Customers Cu, Products P
	WHERE ( ((I.InvoiceDate >= @FromDate) AND (I.InvoiceDate <= @ToDate)) AND
		(Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IPS.InvoiceID) AND (IPS.ProductID = P.ProductID))
RETURN