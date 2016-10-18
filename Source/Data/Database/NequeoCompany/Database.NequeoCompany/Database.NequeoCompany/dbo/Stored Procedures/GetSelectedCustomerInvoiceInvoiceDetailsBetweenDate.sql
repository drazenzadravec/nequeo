CREATE PROCEDURE GetSelectedCustomerInvoiceInvoiceDetailsBetweenDate
	@FromDate datetime, @ToDate datetime, @CustomerID int
AS
	SELECT I.*, IDS.*, Cu.*
	FROM Invoices I, InvoiceDetails IDS, Customers Cu
	WHERE ( ((I.InvoiceDate >= @FromDate) AND (I.InvoiceDate <= @ToDate)) AND
		 (Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IDS.InvoiceID))
RETURN