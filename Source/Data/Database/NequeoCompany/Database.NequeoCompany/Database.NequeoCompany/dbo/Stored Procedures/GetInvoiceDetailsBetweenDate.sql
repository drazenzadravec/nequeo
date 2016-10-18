CREATE PROCEDURE [dbo].[GetInvoiceDetailsBetweenDate]
	@FromDate datetime, @ToDate datetime
AS
	--DECLARE @FromDay char(2) , @FromMonth char(2), @FromYear char(4), @From char(8)
	--DECLARE @ToDay char(2) , @ToMonth char(2), @ToYear char(4), @To char(8)

	--SET @FromDay = CAST(DATEPART(dd, @FromDate) as char(2))
	--SET @FromMonth = CAST(DATEPART(mm, @FromDate) as char(2))
	--SET @FromYear = CAST(DATEPART(yyyy, @FromDate) as char(4))
	--SET @From = @FromYear+@FromMonth+@FromDay

	--SET @ToDay = CAST(DATEPART(dd, @ToDate) as char(2))
	--SET @ToMonth = CAST(DATEPART(mm, @ToDate) as char(2))
	--SET @ToYear = CAST(DATEPART(yyyy, @ToDate) as char(4))
	--SET @To = @FromYear+@FromMonth+@FromDay

	SELECT I.InvoiceID, I.InvoiceDate,  I.IncomeType, C.CustomerID, C.CompanyName, 
		Totals = SUM(IDS.Total) + SUM(IDS.GST), PreGST = Round((SUM(IDS.Total) + SUM(IDS.GST)) - ((SUM(IDS.Total) + SUM(IDS.GST)) / [dbo].[GetGSTMultiplierValue]()), 2)
	FROM Invoices I, InvoiceDetails IDS, Customers C
	WHERE ( (I.InvoiceDate Between @FromDate AND @ToDate)
		 AND (I.InvoiceID = IDS.InvoiceID) 
		 AND (I.CustomerID = C.CustomerID) )
	GROUP BY I.InvoiceDate, I.InvoiceID,  I.IncomeType, C.CustomerID, C.CompanyName
RETURN