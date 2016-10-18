CREATE PROCEDURE GetInvoiceBetweenDate
	@FromDate datetime, @ToDate datetime
AS
	SELECT I.InvoiceID, I.InvoiceDate, I.PaymentDate, I.IncomeType, C.CustomerID, C.CompanyName
	FROM Invoices I, Customers C
	WHERE ( (I.InvoiceDate >= @FromDate) AND (I.InvoiceDate <= @ToDate)
		 AND (I.CustomerID = C.CustomerID) )
	GROUP BY I.InvoiceDate, I.InvoiceID, I.PaymentDate,  I.IncomeType, C.CustomerID, C.CompanyName
RETURN