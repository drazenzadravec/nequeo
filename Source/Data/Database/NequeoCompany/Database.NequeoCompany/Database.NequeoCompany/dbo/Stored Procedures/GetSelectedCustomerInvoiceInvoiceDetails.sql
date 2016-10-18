CREATE PROCEDURE GetSelectedCustomerInvoiceInvoiceDetails
	@CustomerID int
AS
	SELECT I.*, IDS.*, Cu.*
	FROM Invoices I, InvoiceDetails IDS, Customers Cu
	WHERE ( (Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IDS.InvoiceID))
RETURN