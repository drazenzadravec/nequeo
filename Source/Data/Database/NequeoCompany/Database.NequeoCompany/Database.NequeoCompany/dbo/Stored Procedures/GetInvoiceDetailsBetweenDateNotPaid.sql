CREATE PROCEDURE [dbo].[GetInvoiceDetailsBetweenDateNotPaid]
	@FromDate datetime, @ToDate datetime
AS
	SELECT I.InvoiceID, I.InvoiceDate,  I.IncomeType, C.CustomerID, C.CompanyName, 
		Totals = SUM(IDS.Total) + SUM(IDS.GST), PreGST = Round((SUM(IDS.Total) + SUM(IDS.GST)) - ((SUM(IDS.Total) + SUM(IDS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceDetails IDS, Customers C
	WHERE ( (I.InvoiceDate Between @FromDate AND @ToDate) AND (I.PaymentDate = null)
		 AND (I.InvoiceID = IDS.InvoiceID) 
		 AND (I.CustomerID = C.CustomerID) )
	GROUP BY I.InvoiceDate, I.InvoiceID,  I.IncomeType, C.CustomerID, C.CompanyName
RETURN