CREATE PROCEDURE [dbo].[GetSelectedCustomerInvoiceInvoiceProductsTotalsBetweenDate]
	@FromDate datetime, @ToDate datetime, @CustomerID int
AS
	SELECT SumTotal = Sum(IPS.Total) + SUM(IPS.GST), SumGST =  Round((SUM(IPS.Total) + SUM(IPS.GST)) - ((SUM(IPS.Total) + SUM(IPS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceProducts IPS, Customers Cu, Products P
	WHERE ( ((I.InvoiceDate >= @FromDate) AND (I.InvoiceDate <= @ToDate)) AND
		(Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IPS.InvoiceID) AND (IPS.ProductID = P.ProductID))
RETURN