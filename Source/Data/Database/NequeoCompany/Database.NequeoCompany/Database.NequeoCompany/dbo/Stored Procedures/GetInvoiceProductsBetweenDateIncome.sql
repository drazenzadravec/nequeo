CREATE PROCEDURE [dbo].[GetInvoiceProductsBetweenDateIncome]
	@FromDate datetime, @ToDate datetime
AS
	SELECT I.InvoiceID, I.InvoiceDate, I.PaymentDate,  I.IncomeType, C.CustomerID, C.CompanyName, 
		Totals = SUM(IPS.Total) + SUM(IPS.GST), PreGST = Round((SUM(IPS.Total) + SUM(IPS.GST)) - ((SUM(IPS.Total) + SUM(IPS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceProducts IPS, Customers C
	WHERE (  ((I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate))
		 AND (I.InvoiceID = IPS.InvoiceID) 
		 AND (I.CustomerID = C.CustomerID) )
	GROUP BY I.InvoiceDate, I.InvoiceID, I.PaymentDate,  I.IncomeType, C.CustomerID, C.CompanyName
RETURN