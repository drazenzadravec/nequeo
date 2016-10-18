CREATE PROCEDURE GetSelectedInvoiceProducts
	@InvoiceID int, @CompanyID int, @AccountID int
AS
	SELECT C.*, A.*, I.*, IPS.*, Cu.*
	FROM Companies C, Accounts A, Invoices I, InvoiceProducts IPS, Customers Cu
	WHERE ((C.CompanyID = @CompanyID) AND 
		(A.AccountID = @AccountID) AND 
		(I.InvoiceID = @InvoiceID) AND 
		(IPS.InvoiceID = @InvoiceID) AND 
		(I.CustomerID = Cu.CustomerID))
RETURN