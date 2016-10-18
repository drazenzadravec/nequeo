CREATE PROCEDURE [dbo].[GetSelectedCustomerInvoiceInvoiceDetailsTotalsBetweenDate]
	@FromDate datetime, @ToDate datetime, @CustomerID int
AS
	SELECT SumTotal = Sum(IDS.Total) + SUM(IDS.GST), SumGST = Round((SUM(IDS.Total) + SUM(IDS.GST)) - ((SUM(IDS.Total) + SUM(IDS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceDetails IDS, Customers Cu
	WHERE ( ((I.InvoiceDate >= @FromDate) AND (I.InvoiceDate <= @ToDate)) AND
		 (Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IDS.InvoiceID))
RETURN