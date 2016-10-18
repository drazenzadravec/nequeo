CREATE PROCEDURE [dbo].[GetBASInvoiceDetailsBetweenDate]
	@FromDate datetime, @ToDate datetime
AS
	SELECT Totals = ROUND((SUM(IDS.Total) + SUM(IDS.GST)), 0), PreGST = Round((SUM(IDS.Total) + SUM(IDS.GST)) - ((SUM(IDS.Total) + SUM(IDS.GST)) / [dbo].[GetGSTMultiplierValue]()), 0)
	FROM Invoices I, InvoiceDetails IDS
	WHERE   (( (I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate) )
		AND  ( I.InvoiceID = IDS.InvoiceID) )
RETURN