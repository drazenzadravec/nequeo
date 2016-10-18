CREATE PROCEDURE [dbo].[GetSelectedCustomerInvoiceInvoiceProductsTotals]
	@CustomerID int
AS
	SELECT SumTotal = Sum(IPS.Total) + SUM(IPS.GST), SumGST =  Round((SUM(IPS.Total) + SUM(IPS.GST)) - ((SUM(IPS.Total) + SUM(IPS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceProducts IPS, Customers Cu, Products P
	WHERE ( (Cu.CustomerID = @CustomerID) AND (I.CustomerID =  @CustomerID) AND (I.InvoiceID = IPS.InvoiceID) AND (IPS.ProductID = P.ProductID))
RETURN