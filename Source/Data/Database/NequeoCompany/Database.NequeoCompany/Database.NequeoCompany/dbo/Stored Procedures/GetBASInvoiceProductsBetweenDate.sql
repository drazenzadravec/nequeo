CREATE PROCEDURE [dbo].[GetBASInvoiceProductsBetweenDate]
	@FromDate datetime, @ToDate datetime
AS
	SELECT Totals = ROUND((SUM(IPS.Total) + SUM(IPS.GST)), 0), PreGST = Round((SUM(IPS.Total) + SUM(IPS.GST)) - ((SUM(IPS.Total) + SUM(IPS.GST)) / [dbo].[GetGSTMultiplierValue]()), 0)
	FROM Invoices I, InvoiceProducts IPS
	WHERE   (( (I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate) )
		AND  ( I.InvoiceID = IPS.InvoiceID) )
RETURN