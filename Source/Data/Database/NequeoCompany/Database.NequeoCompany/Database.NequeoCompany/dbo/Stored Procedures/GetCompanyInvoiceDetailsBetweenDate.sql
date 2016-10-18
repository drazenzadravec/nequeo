CREATE PROCEDURE [dbo].[GetCompanyInvoiceDetailsBetweenDate]
	@FromDate datetime, @ToDate datetime
AS
	SELECT Totals = (SUM(IDS.Total) + SUM(IDS.GST)), PreGST = (SUM(IDS.Total) + SUM(IDS.GST)) - ((SUM(IDS.Total) + SUM(IDS.GST)) / [dbo].[GetGSTMultiplierValue]())
	FROM Invoices I, InvoiceDetails IDS
	WHERE   (( (I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate) )
		AND  ( I.InvoiceID = IDS.InvoiceID) )
RETURN