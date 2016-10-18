CREATE PROCEDURE [dbo].[GetInvoiceDetailsSummaryBetweenDateIncome]
	@FromDate datetime, @ToDate datetime
AS
	SELECT Totals = SUM(IDS.Total) + SUM(IDS.GST), PreGST = Round((SUM(IDS.Total) + SUM(IDS.GST)) - ((SUM(IDS.Total) + SUM(IDS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceDetails IDS
	WHERE (  ((I.PaymentDate >= @FromDate) AND (I.PaymentDate <= @ToDate))
		 AND (I.InvoiceID = IDS.InvoiceID) )
RETURN