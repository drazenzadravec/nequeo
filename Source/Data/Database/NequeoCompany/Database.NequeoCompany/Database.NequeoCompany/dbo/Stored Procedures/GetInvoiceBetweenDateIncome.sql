create PROCEDURE [dbo].[GetInvoiceBetweenDateIncome]
	@FromDate datetime, @ToDate datetime
AS
	SELECT I.InvoiceID, I.InvoiceDate, I.PaymentDate, I.IncomeType, C.CustomerID, C.CompanyName
	FROM Invoices I, Customers C
	WHERE ( (I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate)
		 AND (I.CustomerID = C.CustomerID) )
	GROUP BY I.InvoiceDate, I.InvoiceID, I.PaymentDate,  I.IncomeType, C.CustomerID, C.CompanyName
RETURN