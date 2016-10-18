CREATE PROCEDURE [dbo].[GetInvoiceDetailsBetweenDatePayment]
	@FromDate datetime, @ToDate datetime
AS
	SELECT I.InvoiceID, I.InvoiceDate, I.PaymentDate,  I.IncomeType, C.CustomerID, C.CompanyName, 
		Totals = SUM(IDS.Total) + SUM(IDS.GST), PreGST = Round((SUM(IDS.Total) + SUM(IDS.GST)) - ((SUM(IDS.Total) + SUM(IDS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceDetails IDS, Customers C
	WHERE (  ((I.InvoiceDate >= @FromDate) AND (I.InvoiceDate <= @ToDate))
		 AND (I.InvoiceID = IDS.InvoiceID) 
		 AND (I.CustomerID = C.CustomerID) )
	GROUP BY I.InvoiceDate, I.InvoiceID, I.PaymentDate,  I.IncomeType, C.CustomerID, C.CompanyName
RETURN