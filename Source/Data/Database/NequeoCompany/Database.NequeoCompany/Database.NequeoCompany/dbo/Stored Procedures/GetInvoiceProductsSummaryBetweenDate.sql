CREATE PROCEDURE [dbo].[GetInvoiceProductsSummaryBetweenDate]
	@FromDate datetime, @ToDate datetime
AS
	SELECT Totals = SUM(IPS.Total) + SUM(IPS.GST), PreGST = Round((SUM(IPS.Total) + SUM(IPS.GST)) - ((SUM(IPS.Total) + SUM(IPS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceProducts IPS
	WHERE   ( ((I.InvoiceDate >= @FromDate) AND (I.InvoiceDate <= @ToDate))
		AND  ( I.InvoiceID = IPS.InvoiceID) )
RETURN