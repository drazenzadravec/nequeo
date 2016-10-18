CREATE PROCEDURE GetSelectedInvoiceDetails
	@InvoiceID int, @CompanyID int, @AccountID int
AS
	SELECT C.*, A.*, I.*, IDS.*, Cu.*
	FROM Companies C, Accounts A, Invoices I, InvoiceDetails IDS, Customers Cu
	WHERE ((C.CompanyID = @CompanyID) AND 
		(A.AccountID = @AccountID) AND 
		(I.InvoiceID = @InvoiceID) AND 
		(IDS.InvoiceID = @InvoiceID) AND 
		(I.CustomerID = Cu.CustomerID))
RETURN